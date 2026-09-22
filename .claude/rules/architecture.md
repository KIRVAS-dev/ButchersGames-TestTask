---
paths:
  - "ButchersGames/Assets/_Project/Scripts/**/*.cs"
---

# Architecture

Когда читать: правки `ButchersGames/Assets/_Project/Scripts/**/*.cs`; вопросы про слои, DI, эталон фичи.

Суть (Context, Stack, dependency rule топ, soft-checks) — [../../CLAUDE.md](../../CLAUDE.md), §2. Общие практики — §3 там же; стиль — [codestyle.md](codestyle.md); исключения и Guard — [exceptions.md](exceptions.md). Направление потока данных обязательно (см. **Поток данных** ниже).

## Слои и asmdef

```
Assets/_Project/Scripts/
├── Infrastructure/
│   ├── Bootstrap/           Infrastructure.Bootstrap  — ProjectScope, EntryPoint
│   ├── ExtendedExceptions/  ExtendedExceptions        — ExtendedException, Guard
│   └── Persistence/         Infrastructure.Persistence — PlayerPrefs и пр. адаптеры хранения
├── Core/
│   ├── Loop/Api/            Core                      — порты цикла (namespace Core.Loop): IInputTickable, IGameplayTickable, IPresentationTickable
│   ├── Lifecycle/           Core                      — жизненный цикл scope (namespace Core.Lifecycle): CoreScopeCancellationSource; `Api/` — ICoreScopeCancellation, ISubscriptionLifecycle
│   ├── Bootstrap/           Bootstrap                 — CoreScope, CoreEntryPoint, GameLoop; `Scene/` — CoreLoader + `Api/` ISceneLoader, LoadSceneMode (Core.Bootstrap.Scene)
│   ├── Gameplay/{Feature}/  Core                      — Model, Service, Api (в т.ч. I*Settings)
│   └── Input/               Core                      — порты ввода `Api/` (IDragInput, namespace Core.Input) и `{Feature}/` InputHandler → Service (namespace Core.Input.{Feature})
├── Input/                   Input                     — адаптеры устройств ввода: реализуют порты Core (I*Input, IInputTickable), читают Input System
├── UI/{Screen}/             UI                        — View, Presenter, Config экрана (MVP), см. «UI-экраны»
├── ViewComponents/{Feature}/ ViewComponents           — View, Providers, SO Config, scene MonoBehaviour
├── Debug/                   Debug                     — Editor-only утилиты (`#if UNITY_EDITOR`), без ссылок на gameplay
└── Rendering/               Rendering                 — URP Renderer Features
```

### Зависимости asmdef (направление ссылок)

Стрелка `A ← B` значит **B ссылается на A**.

```
ExtendedExceptions  ←  Core  ←  ViewComponents
Core  ←  Input  ←  Bootstrap  ←  Infrastructure.Bootstrap
Core  ←  Infrastructure.Persistence  ←  Bootstrap
Rendering  — изолирован от gameplay (URP only)
```

| Сборка | Ссылается на | Содержит |
|---|---|---|
| **ExtendedExceptions** | — | `ExtendedException`, `Guard` |
| **Infrastructure.Persistence** | Core | Адаптеры persistence (PlayerPrefs и т.п.) |
| **Core** | ExtendedExceptions, R3 | Gameplay, порты `Api/` (без SO Config), фазы цикла |
| **Input** | Core, Unity.InputSystem | Адаптеры устройств: `DragInput` и т.п. (plain C#, реализуют порты Core), `ITrigger` |
| **ViewComponents** | Core, ExtendedExceptions, UniTask, VContainer, FMOD, Splines, R3 | View, Providers, SO Config |
| **UI** | Core, ExtendedExceptions, DOTween, TMP, uGUI, R3 | Экраны UI: View, Presenter, Config |
| **Bootstrap** | Core, Input, UI, ViewComponents, Infrastructure.Persistence, ExtendedExceptions, VContainer, UniTask | CoreScope, CoreEntryPoint, GameLoop |
| **Debug** | Unity.InputSystem | Editor-only отладка |
| **Infrastructure.Bootstrap** | Bootstrap, VContainer, UniTask | ProjectScope, загрузка Core |
| **Rendering** | URP | Пост-эффекты |

**`Api/` везде.** Интерфейсы (порты), DTO и `Exceptions.cs` любой папки лежат в её подпапке `Api/` — даже если рядом нет реализаций и папка состоит из одних интерфейсов. Namespace без суффикса `.Api`.

VContainer, UniTask, R3 — пакеты; в asmdef вручную не добавлять (кроме явных precompiled, если нужно).

## Dependency rule

| Запрещено | Почему |
|---|---|
| `Core` → `ViewComponents` | Core не знает сцену |
| `Core` → `Input` | Порты ввода принадлежат Core (потребитель); адаптер устройства в `Input` зависит от Core, не наоборот |
| View вызывает `I*Service` напрямую | Обход входного адаптера (InputHandler; для UI-экрана — Presenter) |
| Model использует `UnityEngine.*` | State должен быть pure C# |
| View принимает gameplay-решения / меняет Model | Логика только в Service |
| View напрямую держит ссылку на конкретный `Model`-класс | Между View и Model — Presenter, см. **Model → View через Presenter** |

Разрешено: `ViewComponents` → `Core` (реализует `I*View` / `I*Performer` / `I*Loader` / `I*Provider`); `Input` → `Core` (реализует `I*Input` / `IInputTickable`); Bootstrap регистрирует конкретные View в DI.

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
| `CoreEntryPoint` | Core | `ISubscriptionLifecycle.Start()` у всех зарегистрированных реализаций (в любом порядке), затем `IGameFlowService.PrepareGame()` — подготовка игры после всех подписок | `ISubscriptionLifecycle.Stop()` |
| `GameLoop` | Core | — (только `ITickable`) | — |

`RegisterEntryPoint<T>()` в VContainer — это способ получить колбэки жизненного цикла (`IStartable`, `ITickable`, `IDisposable`), а не отдельное архитектурное понятие. В Core-scope их две:

- `CoreEntryPoint` (`RegisterEntryPoint<CoreEntryPoint>()`) — **единственный стартер** Core-геймплея: через DI получает `IReadOnlyList<ISubscriptionLifecycle>` и `IGameFlowService`, не перечисляет concrete Presenter/сервисы в ctor.
- `GameLoop` (`RegisterEntryPoint<GameLoop>()`) — только тик кадра. Через DI получает списки всех реализаций `IInputTickable` (чтение ввода), `IGameplayTickable` (симуляция, `Tick(deltaTime)`) и `IPresentationTickable` (применение итога кадра к сцене) и на каждом кадре обходит их в этом порядке: «ввод → геймплей → отображение». Отдельных классов-фаз нет: реализация регистрируется как `.As<I*Tickable>()` (например, `DragInput` — `IInputTickable`, `RunnerMovementService` — `IGameplayTickable`, `RunnerMovementView` — `IPresentationTickable`). Для View, которому дорого применять каждое изменение, `Set*` от Presenter только запоминают значения, а `IPresentationTickable.Tick()` применяет их один раз за кадр. GameLoop не запускает фичи и не содержит логики. `IGameplayInputBlock` — флаг блокировки геймплейного ввода, `GameplayInputBlock` вычисляет его из `GameStateModel.State` (заблокирован везде, кроме `Run`), читает InputHandler.

Новый участник цикла реализует `I*Tickable` и регистрируется через `.As<I*Tickable>()`, а не отдельными `ITickable`/`RegisterEntryPoint`. Новая фича с долгоживущими подписками (InputHandler, Presenter, сервис-наблюдатель без потребителя через ctor — как `GameResultDetector`, `WealthPointsModifierService`) — реализует `ISubscriptionLifecycle` (`Start`/`Stop`), регистрируется `.As<ISubscriptionLifecycle>()`; **не** получает свой `IStartable`/`RegisterEntryPoint`/`RegisterBuildCallback`. `CoreEntryPoint` обходит весь список сам. Единый паттерн на все такие типы.

## Эталон фичи `{Feature}`

Новые фичи повторяют эту анатомию.

### Структура файлов

```
Core/Gameplay/{Feature}/
├── Api/
│   ├── I{Feature}Service.cs
│   ├── I{Feature}View.cs          — порт; реализации во ViewComponents
│   ├── I{Feature}Provider.cs      — опционально
│   ├── I{Feature}Settings.cs      — pure C# порт настроек (если нужен Config)
│   ├── {Feature}*Data.cs          — DTO
│   └── Exceptions.cs
├── {Feature}Model.cs
├── {Feature}Service.cs            — зависит от I{Feature}Settings, не от SO
└── {Feature}*Registry.cs          — опционально

Core/Input/{Feature}/
└── {Feature}InputHandler.cs

ViewComponents/{Feature}/
├── Api/
│   ├── {Feature}Config.cs         — ScriptableObject : I{Feature}Settings (если нужен)
│   └── Exceptions.cs              — view/Inspector ошибки
├── {Feature}View.cs               — : I{Feature}View
└── {Feature}*Provider.cs          — : I{Feature}Provider (если нужен)
```

### Роли

| Роль | Где | Обязанности |
|---|---|---|
| **Model** | `Core/Gameplay/{Feature}/` | Mutable state. Без side effects. Без `UnityEngine.*` |
| **Service** | `Core/Gameplay/{Feature}/` | Валидация, оркестрация, `ExtendedException`. Единственная точка вызова извне (`I{Feature}Service`) |
| **Settings** | `Core/Gameplay/{Feature}/Api/` | Pure C# порт `I{Feature}Settings` — геттеры без Unity |
| **Config** | `ViewComponents/{Feature}/Api/` | `[CreateAssetMenu]` ScriptableObject, реализует `I{Feature}Settings`. `Validate()` здесь |
| **Registry** | `Core/Gameplay/{Feature}/` | Индекс / lookup по данным Provider |
| **Api/** | `Core/Gameplay/{Feature}/Api/` | Интерфейсы, DTO, `Exceptions.cs` |
| **View** | `ViewComponents/{Feature}/` | Отображает **одну конкретную сущность или экран** сцены (персонаж, уровень, HUD) и реализует `I{Feature}View`. DOTween, VFX, Animator. Не меняет game state |
| **Performer** | `ViewComponents/{Feature}/` | Один на сцену, без своей сущности: принимает смысловой тип события (`enum`) и проигрывает эффекты по таблице записей (звук FMOD, VFX). Реализует `I{Feature}Performer` из Core Api. Не меняет game state. Пример: `FeedbackPerformer` |
| **Loader** | `ViewComponents/{Feature}/` | Один на сцену: по команде Core (`Load{X}(index)`) создаёт в сцене экземпляр контента (спавн префаба из своего конфига), хранит его как текущий и сообщает о готовности событием `{X}Loaded`; отдаёт Core счётчик и настройки загружаемого списка. Реализует `I{Feature}Loader` из Core Api. Не меняет game state. Содержимое загруженного экземпляра он передаёт в постоянный класс `Current{X}` (обычный C#, Singleton в DI), который хранит его и сам реализует порты `I*Registry`/`I*Provider` для Core; читатели зависят от `Current{X}`, а не от Loader. Примеры: `LevelLoader` + `CurrentLevel` |
| **Provider** | `ViewComponents/{Feature}/` | Сцена → DTO / данные для Core через порт |
| **InputHandler** | `Core/Input/{Feature}/` | Порт ввода Core (`I*Input`) → `I{Feature}Service`. Без ссылок на ViewComponents и Input |
| **Input adapter** | `Input/` | Plain C# адаптер устройства: реализует порт Core (`I*Input`, `IInputTickable`), читает Input System. Регистрируется `Register<T>(Singleton).As<…>()` |
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
    builder.RegisterInstance<IFeatureSettings>(_featureConfig);
    builder.RegisterInstance(_featureProvider).As<IFeatureProvider>();
    builder.RegisterComponentInHierarchy<FeatureView>().As<IFeatureView>();
    builder.Register<FeatureRegistry>(Lifetime.Singleton).As<IFeatureRegistry>();
    builder.Register<FeatureModel>(Lifetime.Singleton);
    builder.Register<FeatureService>(Lifetime.Singleton).As<IFeatureService>();
    builder.Register<FeatureInputHandler>(Lifetime.Singleton);
}
```

Правила:

- **Config** (SerializeField на CoreScope, тип из ViewComponents) → `RegisterInstance<I*Settings>(so)`; Provider — `.As<I*Provider>()`
- **View на сцене** → `RegisterComponentInHierarchy<TView>().As<I*View>()`
- **Model, Service, Registry, InputHandler** → `Register<T>(Lifetime.Singleton)`; Service — `.As<I*Service>()`
- Core **не** ссылается на concrete Config SO — только на `I*Settings`

## Как добавить фичу

1. `Core/Gameplay/{Feature}/Api/` — `I{Feature}Service`, порты View/Provider/Settings, DTO, `Exceptions.cs`
2. `Core/Gameplay/{Feature}/` — Model, Service, Registry при необходимости
3. `ViewComponents/{Feature}/` — View, Providers, `{Feature}Config` SO; `Api/Exceptions.cs` для view-ошибок
4. `Core/Input/{Feature}/` — InputHandler, если есть низкоуровневый ввод без экрана (drag, клавиши). Ввод UI-экрана (клики кнопок) — через Presenter, см. **UI-экраны (GameUI, MVP)**
5. `CoreScope` — `Register{Feature}`, SerializeField для config/providers
6. `CoreEntryPoint` — `ISubscriptionLifecycle.Start()`/`Stop()` (через `IReadOnlyList<>`) + `PrepareGame()`; регистрация `.As<ISubscriptionLifecycle>()`
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

**Не предлагать:** ECS, System Groups, Event Queue, Command Bus, CQRS, Presenter на каждую геймплей-сущность, которая уже общается через event-порты (`I*Provider`/`ITriggerReaction` и т.п.), а не через `Model` (Model/Service/View — умолчание для таких сущностей: пикапы, препятствия, `WealthPointsModifierCollider`, `Obstacle`). Presenter обязателен только там, где View иначе пришлось бы напрямую держать ссылку на конкретный `Model`-класс — см. **Model → View через Presenter** ниже.

## Model → View через Presenter

View **никогда** не держит прямую ссылку на конкретный `Model`-класс — ни своей фичи, ни тем более чужой. Если View должен реагировать на изменение `Model` (не Provider/Service-порта, а именно `*Model`), между ними обязателен **Presenter** (plain C#, не `MonoBehaviour`): ctor DI на нужный `*Model` + узкий пассивный `I*View`-порт, реализуемый View. Presenter R3-подпиской транслирует изменения `Model` в вызовы `I*View` (`Show`/`Hide`/`SetX(value)`); View только применяет вызов, сам не подписывается и не решает.

Это не ограничено UI-экранами — тот же паттерн для любого View, у которого нет своей `Model`, но есть чужая (примеры: `CharacterAppearanceView` реагирует на `WealthMeterModel.Stage` через `CharacterAppearancePresenter`; `RunnerTrackFollower` — на `RunnerMovementModel.LateralOffset`/`State` через `RunnerMovementPresenter`). Легитимные Core-порты (`I*Service`/`I*Provider`), не относящиеся к `Model`, остаются в самом View/InputHandler как обычно — через Presenter выносится только то, что иначе стало бы прямой ссылкой на `Model`.

Presenter — обычный `Register<TPresenter>(Lifetime.Singleton).As<ISubscriptionLifecycle>()` в `CoreScope`, без своего `I*Service` в Core Api. Лайфцикл подписок — `ISubscriptionLifecycle` (формирует eager-резолв через список в `CoreEntryPoint`, см. **Entry points**).

### Роль Presenter — классический MVP

Presenter — презентер в классическом смысле MVP: он владеет **логикой представления**. Подписывается на Core-состояние (один или несколько `*Model`), сам решает, что View должна показать или проиграть, и отдаёт ей готовую команду (`Show`/`Hide`/`SetX(value)`/`Play(slot)`).

| Кто | Что делает | Чего не делает |
|---|---|---|
| **Presenter** | Маппинг Core-состояния в команду View: `switch` по `GameState`, свёртка нескольких `*Model` в один результат, выбор слота/варианта, форматирование значения | Gameplay-решений: не меняет `Model`, не валидирует, не определяет исход игры — это Service. Core сообщает, **что случилось** (`Win`, `Stopped`), Presenter решает, **что показать** |
| **View** | Применяет команду | Не читает Core, не подписывается, не выбирает, что показывать |

- Если Presenter вынужден угадывать причину по косвенным признакам, недостающий факт добавляется в Core как событие/порт (например `IObstacle.Hit`), а не выдумывается новым значением состояния под нужды отображения (`RunnerMovementState` остаётся `Moving`/`Stopped`)
- Один Presenter может свести несколько `*Model` в одну команду View
- Примеры: `CharacterAnimationPresenter` (`GameStateModel` + `RunnerMovementModel` → `CharacterAnimationSlot` → `ICharacterAnimationView.Play`), `FeedbackPresenter` (`GameState` → `FeedbackType`), `CharacterAppearancePresenter` (`WealthMeterModel.Stage` → `SetActiveStage`)

Частный случай — UI-экран без собственной модели вообще: там Presenter/View дополнительно разносятся по отдельным файлам с фиксированной структурой `UI/{Screen}/`, см. ниже.

## UI-экраны (GameUI, MVP)

Экран UI (старт/HUD/результат и т.п.) — частный случай **Model → View через Presenter** выше: своей игровой модели нет, только отображение уже существующего Core-состояния (`GameStateModel`, `WealthMeterModel` и т.п. других фич). Для экранов конкретно — фиксированная структура файлов и контракт `Show`/`Hide`/`SetX` ниже.

### Структура файлов

```
UI/{Screen}/
├── Api/
│   ├── I{Screen}View.cs        — пассивный контракт: Show()/Hide()/SetX(value) + `event Action {Action}Clicked` для кнопок, без R3, без условий
│   └── Exceptions.cs           — view/Inspector-ошибки, как у обычного View
├── {Screen}View.cs             — : MonoBehaviour, I{Screen}View — только применяет вызовы, ничего не решает
└── {Screen}Presenter.cs        — plain C# (не MonoBehaviour)
```

### Роли

| Роль | Где | Обязанности |
|---|---|---|
| **Presenter** | `UI/{Screen}/` | ctor DI на существующий Core `*Model`/`I*Service` (не новая модель для самого экрана) + `I{Screen}View`. R3-подпиской транслирует Core-состояние в вызовы `I{Screen}View` (`Show`/`Hide`/`SetX`); подписан на `{Action}Clicked` View и вызывает `I*Service` — единственная точка ввода экрана |
| **View** | `UI/{Screen}/` | Реализует `I{Screen}View`. `SetActive`/`Set*` на UI-элементах; клик uGUI `Button` пробрасывает как `event Action` — без чтения Core, без R3, без условий, без вызова Service |

**Ввод UI-экрана.** Клик кнопки: `Button.onClick` → View поднимает `{Action}Clicked` → Presenter вызывает `I*Service` (например `IGameFlowService.StartGame()`). Отдельный `InputHandler`, порт ввода в `Core/Input/Api` и `ITrigger`-обёртка для кнопок экрана не нужны. Presenter только передаёт намерение в Service и **не принимает gameplay-решений** — валидация допустимости (`Guard`, переходы состояний) остаётся в Service. `InputHandler` (`Core/Input/{Feature}/`) остаётся для низкоуровневого ввода без экрана (drag, клавиши).

Регистрация и лайфцикл Presenter — как описано в **Model → View через Presenter** выше.

**Не размножать** структуру `UI/{Screen}/` (Presenter + `I{Screen}View` с фиксированным неймингом экрана) на геймплей-сущности, которые уже общаются через event-порты, а не `Model` (пикапы, препятствия — `WealthPointsModifierCollider`, `Obstacle`) — там остаётся обычная пара Model+Service (Core) / View (ViewComponents), см. **Model / View** выше и **Эталон фичи**. Presenter из-за прямого чтения `Model` (не привязанный к структуре экрана) — см. **Model → View через Presenter** выше; пример вне UI-экранов — `CharacterAppearanceView`/`RunnerTrackFollower`.

## Architectural constraints

- Soft-checks запрещены — секция выше
- Бизнес-логика только в `Core/Gameplay/{Feature}/` (Service)
- Core **не** ссылается на ViewComponents; View реализует интерфейсы из Core
- Core (включая InputHandler) **не** ссылается на Input и ViewComponents
- ViewComponents **не** вызывает Service напрямую — только через InputHandler; Providers отдают данные, не команды
- UI-экран (`UI/{Screen}/`): View Service не вызывает, клики уходят `event Action` → Presenter → `I*Service`
