# Architecture

Когда читать: правки `Template-UnityProject/Assets/_Project/Scripts/**/*.cs`; вопросы про слои, DI, эталон фичи.

Суть (Context, Stack, dependency rule топ, soft-checks) — [../../CLAUDE.md](../../CLAUDE.md), §2. Общие практики — §3 там же; стиль — [codestyle.md](codestyle.md); исключения и Guard — [exceptions.md](exceptions.md). Направление потока данных обязательно (см. **Поток данных** ниже).

## Слои и asmdef

```
Assets/_Project/Scripts/
├── Infrastructure/
│   ├── Bootstrap/           Infrastructure.Bootstrap  — ProjectScope, EntryPoint
│   └── ExtendedExceptions/  ExtendedExceptions        — ExtendedException, Guard
├── Core/
│   ├── Bootstrap/           Bootstrap                 — CoreScope, CoreEntryPoint, scene loader API
│   ├── Gameplay/{Feature}/  Core                      — Model, Service, Config, Api
│   └── Input/{Feature}/     Core.Input                — InputHandler → Service
├── Input/                   Input                     — feature-agnostic низкоуровневый ввод
├── ViewComponents/{Feature}/ ViewComponents           — View, Providers, scene MonoBehaviour
└── Rendering/               Rendering                 — URP Renderer Features
```

### Зависимости asmdef (направление ссылок)

Стрелка `A ← B` значит **B ссылается на A**.

```
ExtendedExceptions  ←  Core  ←  ViewComponents
                    ←  Input  ←  ViewComponents
Core  ←  Core.Input  ←  Bootstrap  ←  Infrastructure.Bootstrap
Input  ←  Core.Input
Rendering  — изолирован от gameplay (URP only)
```

| Сборка | Ссылается на | Содержит |
|---|---|---|
| **ExtendedExceptions** | — | `ExtendedException`, `Guard` |
| **Core** | ExtendedExceptions, VContainer, UniTask, R3 | Gameplay, порты `Api/` |
| **Input** | (минимально, напр. uGUI) | Низкоуровневый ввод **без** знания фич |
| **ViewComponents** | Core, Input, ExtendedExceptions, UniTask, DOTween | View, Providers |
| **Core.Input** | Core, Input, ExtendedExceptions, UniTask | `{Feature}InputHandler` |
| **Bootstrap** | Core, Core.Input, ViewComponents, VContainer, UniTask | CoreScope, CoreEntryPoint |
| **Infrastructure.Bootstrap** | Bootstrap, VContainer, UniTask | ProjectScope, загрузка Core |
| **Rendering** | URP | Пост-эффекты |

VContainer, UniTask, R3 — пакеты; в asmdef вручную не добавлять (кроме явных precompiled, если нужно).

## Dependency rule

| Запрещено | Почему |
|---|---|
| `Core` → `ViewComponents` | Core не знает сцену |
| `Core.Input` → `ViewComponents` | Handler зависит только от портов Core и низкого Input |
| View вызывает `I*Service` напрямую | Обход входного адаптера |
| Model использует `UnityEngine.*` | State должен быть pure C# |
| View принимает gameplay-решения / меняет Model | Логика только в Service |

Разрешено: `ViewComponents` → `Core` (реализует `I*View` / `I*Provider`); `Core.Input` → `Core` + `Input`; Bootstrap регистрирует конкретные View в DI.

## DI scopes

Наследники `VContainer.Unity.LifetimeScope`:

| Scope | Сцена | Configure |
|---|---|---|
| **ProjectScope** | Bootstrap | EntryPoint, loader сцены (`ISceneLoader` и аналоги) |
| **CoreScope** | Core | Gameplay-фичи, `CoreEntryPoint` |

**Parent binding (только Inspector, без `EnqueueParent`):**

- `ProjectScope` на Bootstrap EntryPoint
- `CoreScope` — Parent Reference → Type = `ProjectScope`
- Загрузка Core: additive из Bootstrap EntryPoint

**Запрещено:** `GameLifetimeScope`, `LifetimeScope.EnqueueParent`.

### Entry points

| EntryPoint | Scope | Start | Dispose |
|---|---|---|---|
| Infrastructure Bootstrap EntryPoint | Project | additive load Core | — |
| `CoreEntryPoint` | Core | `StartListening()` у InputHandler фич | `StopListening()` |

Новая фича с вводом: подключить handler в `CoreEntryPoint` (Start/Dispose).

## Эталон фичи `{Feature}`

Новые фичи повторяют эту анатомию.

### Структура файлов

```
Core/Gameplay/{Feature}/
├── Api/
│   ├── I{Feature}Service.cs
│   ├── I{Feature}View.cs          — порт; реализации во ViewComponents
│   ├── I{Feature}Provider.cs      — опционально
│   ├── {Feature}*Data.cs          — DTO
│   └── Exceptions.cs
├── {Feature}Model.cs
├── {Feature}Service.cs
├── {Feature}*Registry.cs          — опционально
└── {Feature}Config.cs             — опционально, ScriptableObject

Core/Input/{Feature}/
└── {Feature}InputHandler.cs

ViewComponents/{Feature}/
├── Api/Exceptions.cs              — view/Inspector ошибки
├── {Feature}View.cs               — : I{Feature}View
└── {Feature}*Provider.cs          — : I{Feature}Provider (если нужен)
```

### Роли

| Роль | Где | Обязанности |
|---|---|---|
| **Model** | `Core/Gameplay/{Feature}/` | Mutable state. Без side effects. Без `UnityEngine.*` |
| **Service** | `Core/Gameplay/{Feature}/` | Валидация, оркестрация, `ExtendedException`. Единственная точка вызова извне (`I{Feature}Service`) |
| **Config** | `Core/Gameplay/{Feature}/` | `[CreateAssetMenu]` ScriptableObject |
| **Registry** | `Core/Gameplay/{Feature}/` | Индекс / lookup по данным Provider |
| **Api/** | `Core/Gameplay/{Feature}/Api/` | Интерфейсы, DTO, `Exceptions.cs` |
| **View** | `ViewComponents/{Feature}/` | Реализует `I{Feature}View`. DOTween, VFX, Animator. Не меняет game state |
| **Provider** | `ViewComponents/{Feature}/` | Сцена → DTO / данные для Core через порт |
| **InputHandler** | `Core/Input/{Feature}/` | Низкий Input → `I{Feature}Service`. Без ссылок на ViewComponents |
| **Scope** | `Core/Bootstrap/CoreScope` | `Register{Feature}(builder)` |

### Поток данных

```
[низкоуровневый Input]
       ↓
{Feature}InputHandler
       ↓
I{Feature}Service  (Service)
       ↓
Model / Registry  ←  I{Feature}Provider (реализация во View)
       ↓
I{Feature}View     (реализация во View: анимация / VFX)
```

R3: Model → View подписки живут во View или в тонком binder, **без** переноса бизнес-правил во View.

### Регистрация в CoreScope

```csharp
private void RegisterFeature(IContainerBuilder builder)
{
    builder.RegisterInstance(_featureConfig);
    builder.RegisterInstance(_featureProvider).As<IFeatureProvider>();
    builder.RegisterComponentInHierarchy<FeatureView>().As<IFeatureView>();
    builder.Register<FeatureRegistry>(Lifetime.Singleton).As<IFeatureRegistry>();
    builder.Register<FeatureModel>(Lifetime.Singleton);
    builder.Register<FeatureService>(Lifetime.Singleton).As<IFeatureService>();
    builder.Register<FeatureInputHandler>(Lifetime.Singleton);
}
```

Правила:

- **Config / Provider** (SerializeField на CoreScope) → `RegisterInstance`; Provider — `.As<I*Provider>()`
- **View на сцене** → `RegisterComponentInHierarchy<TView>().As<I*View>()`
- **Model, Service, Registry, InputHandler** → `Register<T>(Lifetime.Singleton)`; Service — `.As<I*Service>()`

## Как добавить фичу

1. `Core/Gameplay/{Feature}/Api/` — `I{Feature}Service`, порты View/Provider, DTO, `Exceptions.cs`
2. `Core/Gameplay/{Feature}/` — Model, Service, Config/Registry при необходимости
3. `ViewComponents/{Feature}/` — View, Providers; `Api/Exceptions.cs` для view-ошибок
4. `Core/Input/{Feature}/` — InputHandler, если есть пользовательский ввод
5. `CoreScope` — `Register{Feature}`, SerializeField для config/providers
6. `CoreEntryPoint` — Start/Dispose для handler
7. Сцена Core — View, Providers; ссылки на CoreScope

Проверка на Core: happy path + edge cases (busy, cancel, invalid data → exception).

## Soft-checks

Bad data и невалидная конфигурация → `ExtendedException` (обычно через `Guard` — см. [exceptions.md](exceptions.md)).

**Запрещено:**

- soft `return` / early-exit без throw при битых данных
- `LogWarning` / `Debug.LogWarning` (и аналоги) **вместо** исключения
- «проглотить ошибку и продолжить» в Service / Validate / Provider

UX-гейты во InputHandler (например busy → игнор клика) — не soft-check на bad data; это допустимое поведение входа.

## Model / View

| Слой | Правила |
|---|---|
| **Model** | Pure C# state. Без `UnityEngine.*` |
| **Service** | Вся бизнес-логика фичи. Bad data — по **Soft-checks** |
| **View** | Отображение. `Awake` → `Validate()` через Guard — см. [exceptions.md](exceptions.md) |
| **InputHandler** | Тонкий адаптер. Guard только на UX-гейты (busy и т.п.) |
| **Provider** | Читает сцену, отдаёт данные в Core. Конфиг-ошибки → view-исключения |

**Не предлагать:** ECS, System Groups, Event Queue, Command Bus, CQRS, Presenter на каждую сущность.

## Architectural constraints

- Soft-checks запрещены — секция выше
- Бизнес-логика только в `Core/Gameplay/{Feature}/` (Service)
- Core **не** ссылается на ViewComponents; View реализует интерфейсы из Core
- Core.Input **не** ссылается на ViewComponents
- ViewComponents **не** вызывает Service напрямую — только через InputHandler; Providers отдают данные, не команды
