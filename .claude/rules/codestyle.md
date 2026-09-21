---
paths:
  - "ButchersGames/Assets/_Project/Scripts/**/*.cs"
---

# C# Code Style

Когда читать: `ButchersGames/Assets/_Project/Scripts/**/*.cs`.

Кодстайл WebGL-Template. Principles — [../../CLAUDE.md](../../CLAUDE.md) §3. Exceptions — [exceptions.md](exceptions.md). Слои — [architecture.md](architecture.md). Class / method / variable design — соответствующие файлы: [class-design.md](class-design.md), [method-design.md](method-design.md), [variable-design.md](variable-design.md).

## Namespace

Путь от корня `Scripts/` **без** префикса `_Project.`. **Block-scoped:** `namespace X { }`, не file-scoped.

| Слой | Паттерн | Пример |
|---|---|---|
| Core Gameplay | `Core.Gameplay.{Feature}` | `Core.Gameplay.Movement` |
| Core Bootstrap | `Core.Bootstrap` | `Core.Bootstrap` |
| Core Input | `Core.Input.{Feature}` | `Core.Input.Movement` |
| ViewComponents | `ViewComponents.{Feature}` | `ViewComponents.Hud` |
| Input | `Input` | адаптеры устройств ввода |
| Infrastructure Bootstrap | `Infrastructure.Bootstrap` | `Infrastructure.Bootstrap` |
| Infrastructure Persistence | `Infrastructure.Persistence` | `Infrastructure.Persistence` |
| ExtendedExceptions | `ExtendedExceptions` | shared base |
| Rendering | `Rendering` | URP features |

Исключения фичи: namespace `{Layer}.{Feature}`, файл в `Api/` — [exceptions.md](exceptions.md).

## Структура по слоям

Model / Service / View / InputHandler — см. [architecture.md](architecture.md). Здесь только формат типов:

- **Core:** Model pure C#; Service — ctor DI; Api-ports наружу
- **ViewComponents:** MonoBehaviour + `[SerializeField]`; View реализует `I*View` (отображение одной сущности/экрана), Performer реализует `I*Performer` (эффекты по типу события), Loader реализует `I*Loader` (спавн контента в сцене по команде Core, событие о готовности, счётчик и настройки списка)
- **InputHandler:** тонкий адаптер, ctor DI на `I*Service`

## Структура класса

- Порядок: Поля → Конструктор → Свойства → Методы
- Внутри блока: `public` → `protected` → `private`
- **Порядок полей и параметров конструктора:** сначала интерфейсы (`I…`), потом конкретные классы. Порядок параметров совпадает с порядком полей
- События — выше остальных полей
- Модификаторы доступа **явно** (кроме интерфейсов)
- `static` для методов — **только при строгой необходимости** (factory без состояния, `RuntimeInitializeOnLoad`, extension-методы). Приватные хелперы класса — instance, не `static`
- **Математические Helper-классы** — `public static class {Feature}Helper` в `{ViewLayer}/{Feature}/` (view-math); domain-math без Unity presentation — `{CoreLayer}/{Feature}/`. Stateless pure functions. Суффикс `Helper` **разрешён**. Без MonoBehaviour, DOTween и FMOD
- **`[SerializeField]`** — порядок полей **не менять** (Inspector)

После правок `.cs` — `reformat_file` ([rider-mcp.md](rider-mcp.md)). Порядок членов после Rider — доверять результату reorder, не переставлять вручную обратно.

## Именование

- `camelCase` — локальные переменные, параметры
- `_camelCase` — private поля
- `PascalCase` — public/protected поля, свойства, методы, **локальные функции**, типы
- `PascalCase` — `const` (как и остальные члены типа)
- `static readonly` / `readonly` — по правилам полей, не как const
- Bool-методы: `IsX` / `CanX` / `HasX`; `TryX` для try-паттерна
- События — прошедшее время (`Killed`); хендлеры — `OnX`
- Точные имена, без сокращений; коллекции во множественном числе
- Без тавтологии (`Character.Move()`, не `Character.CharacterMove()`)

### Роли имён

| Имя | Где | Смысл |
|---|---|---|
| `I*Service` | Core Api | единственная точка вызова фичи |
| `I*View` | Core Api | порт отображения **одной конкретной сущности или экрана** сцены |
| `I*Performer` | Core Api | порт проигрывания эффектов (звук/VFX) по смысловому типу события; реализация без собственной сущности — `*Performer` во ViewComponents |
| `I*Loader` | Core Api | порт загрузки контента в сцену по команде Core: `Load*`, событие `*Loaded`, счётчик и настройки списка; реализация — `*Loader` во ViewComponents |
| `I*Provider` | Core Api | данные сцены для Core |
| `*InputHandler` | Core (`Core.Input.{Feature}`) | порт ввода → Service |
| `*Helper` | Core / ViewComponents | `public static class`, stateless functions |
| `*Manager` | — | **не вводить** без явного ok пользователя |
| `*Controller` | — | допустим; уточнить у пользователя соответствие роли |

Присвоение в ctor: `_field = field` когда имена совпадают с параметром.

**Идентификаторы:** без спецсимволов и Unicode — мешают части Unity CLI-тулов.

**Избыточность:** в классе `Player` — поле `Score`, не `PlayerScore`. Методы — [method-design.md](method-design.md).

## Enums

- PascalCase для имени enum и значений
- Имя enum — **единственное число** (`WeaponType`, не `WeaponTypes`)
- `[Flags]` — имя во **множественном числе**

## Properties

- Однострочный read-only — expression-bodied (`=>`)
- Простой get/set — `{ get; set; }` или `{ get; private set; }`
- Операция с нетривиальной логикой — метод, не property

## Rider / форматирование

После правок `.cs` — `reformat_file` ([rider-mcp.md](rider-mcp.md)). Не править отступы/переносы вручную в споре с Rider.

- **Скобки обязательны** для `if` / `for` / `foreach` / `while` / `else`
- **Wrap limit** ~130 символов
- Не более **одной** пустой строки подряд
- **`switch`:** пустая строка **между** `case` блоками
- **`#region`** — не использовать
- Named arguments на call site — когда без них легко перепутать соседние параметры

## Комментарии

- Production-код **без** `//` и `///`
- XML-doc не писать
- Исключение: generated code, Unity templates, Editor-only — не трогать стиль генератора

## Типы и `var`

**Явные типы**, **`var` запрещён**.

## Форматирование

- `[SerializeField] private` для Inspector-полей
- Default values inline: `[SerializeField] float duration = 0.2f;`
- Проверка на false: `if (!x)`, не `if (x == false)`
- `if` / `switch` / `for` / `while` — пустая строка **до и после** блока (жёстко)
- Пустая строка между `case` в `switch`
- Группировка логических блоков пустыми строками
- **Параметры конструкторов/методов:** если параметров больше двух — каждый на отдельной строке:

```csharp
public FooService(
    IBar bar,
    IBazService bazService,
    IQuxProvider quxProvider)
{
```

## Содержимое

- Без магических чисел (кроме очевидного `0`); defaults в SerializeField — ok
- DRY — повторяющиеся блоки в методы
- **`foreach`** — default для коллекций
- **`for`** — когда нужен индекс, обратный проход, параллельные массивы
- Имена в циклах — описательные (`interactableTarget`); `i` — только для индекса
- `goto` — не использовать в новом коде
- `switch` по enum — стремиться к **исчерпываемости**
- Вызов event: `handler?.Invoke(...)`, не `handler(...)`
- Attributes — каждый на **отдельной** строке над членом
- LINQ: короткие цепочки; на hot path — не по умолчанию ([../../CLAUDE.md](../../CLAUDE.md) §3)
- Extension methods — редко; static-класс `{Type}Extensions`
- Публичный Api: named type / `record` вместо длинного `ValueTuple`

Дизайн циклов / nesting — [method-design.md](method-design.md).

## `sealed`

- **Обязательно:** `public sealed class … : ExtendedException`
- **Рекомендуется:** новые leaf View / InputHandler
- Service / Model — обычно **без** `sealed`, если не leaf

## Async

- Только **UniTask** / `UniTask<T>` / `UniTaskVoid`; не `Task`
- Суффикс `Async` обязателен для async-методов
- Fire-and-forget: `UniTaskVoid` или `.Forget()` на UniTask
- `async void` запрещён (кроме event-handler с `try`/`catch`)
- `CancellationToken cancellationToken` — последний параметр; пробрасывать вниз

## IDisposable

- **Explicit** `void IDisposable.Dispose()` — cleanup только внутри типа
- **Public** `void Dispose()` — когда dispose вызывается снаружи
- Поле subscription — `IDisposable` где применимо

## Static

- `public static class {Feature}Helper` — pure functions
- `static void Register*(IContainerBuilder)` — DI registration в LifetimeScope
- Static helpers внутри service/logic классов — **не** добавлять

## SerializeField и MonoBehaviour

- `[SerializeField] private` — `_camelCase` для имён полей
- Публичный доступ к serialized data — через properties, не public fields
- Пояснение полю — `[Tooltip("...")]`, не комментарий
