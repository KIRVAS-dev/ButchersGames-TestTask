---
paths:
  - "ButchersGames/Assets/_Project/Scripts/**/*.cs"
---

# C# Code Style

Layers and roles — [architecture.md](architecture.md); classes / methods / variables and assembly visibility — [design.md](design.md); exceptions — [exceptions.md](exceptions.md).

## Namespace

Path from the `Scripts/` root **without** the `_Project.` prefix. **Block-scoped** `namespace X { }`, not file-scoped.

| Layer | Pattern | Example |
|---|---|---|
| Core Gameplay | `Core.Gameplay.{Feature}` | `Core.Gameplay.Movement` |
| Core Bootstrap | `Core.Bootstrap` | |
| Core Input | `Core.Input.{Feature}` | `Core.Input.Movement` |
| ViewComponents | `ViewComponents.{Feature}` | `ViewComponents.Hud` |
| Input | `Input` | device input adapters |
| Infrastructure | `Infrastructure.Bootstrap`, `Infrastructure.Persistence` | |
| ExtendedExceptions | `ExtendedExceptions` | shared base |

Feature exceptions: namespace `{Layer}.{Feature}`, file in `Api/` — [exceptions.md](exceptions.md).

## Class layout

- Order: Fields → Events → Constructor → Properties → Methods; inside a block `public` → `internal` → `protected` → `private`
- Events: own block right after fields, any access modifier
- Fields and ctor parameters: interfaces (`I…`) first, then concrete classes; parameter order matches field order
- Access modifiers **explicit** (except in interfaces); default `internal` — [design.md](design.md)
- Ctor assignment `_field = field` when names match the parameter

## Naming

- `camelCase` — locals, parameters; `_camelCase` — private fields
- `PascalCase` — public / internal / protected fields, properties, methods, **local functions**, types, `const` (never UPPER_SNAKE_CASE)
- `static readonly` / `readonly` — named like fields, not like const
- Bool methods `IsX` / `CanX` / `HasX`; `TryX` for the try pattern
- Events in past tense (`Killed`); handlers `OnX`
- Precise names, no abbreviations; collections plural
- No tautology with the type name: in `Player` — `Score`, not `PlayerScore`; `Character.Move()`, not `Character.CharacterMove()`
- **Identifiers:** no special or Unicode characters — they break some Unity CLI tools

| Name | Where | Meaning |
|---|---|---|
| `I*Service` | Core Api | the feature's only entry point |
| `I*View` | Core Api | display port of **one concrete entity or screen** |
| `I*Performer` | Core Api | effect playback port (sound / VFX) by semantic event type; impl `*Performer` in ViewComponents |
| `I*Loader` | Core Api | content loading port: `Load*`, event `*Loaded`, list count and settings; impl `*Loader` in ViewComponents |
| `I*Provider` | Core Api | scene data for Core |
| `*InputHandler` | Core (`Core.Input.{Feature}`) | input port → Service |
| `*Helper` | Core / ViewComponents | stateless static class, see **Static** |
| `*Manager` | — | **do not introduce** without the user's explicit ok |
| `*Controller` | — | allowed; confirm the role with the user |

## Enums

- PascalCase for the enum name and values; name **singular** (`WeaponType`); `[Flags]` — **plural**
- **Every member gets an explicit int value** (`Poor = 0, Descent = 1, Casual = 2`), never implicit numbering — for new and edited enums
- `switch` over an enum — aim for **exhaustiveness**

## Properties

- One-line read-only — expression-bodied (`=>`)
- Plain get / set — `{ get; set; }` or `{ get; private set; }`
- Operation with non-trivial logic — a method, not a property

## Formatting

Format and lint after `.cs` edits — [rider-mcp.md](rider-mcp.md). Trust Rider's indentation, wrapping and member order; do not revert it by hand.

- **Braces required** for `if` / `for` / `foreach` / `while` / `else`
- **Wrap limit** ~130 chars; at most **one** blank line in a row
- Blank line **before and after** `if` / `switch` / `for` / `while` blocks; blank line **between** `case` blocks; group logical blocks with blank lines
- **No `#region`**
- False check: `if (!x)`, not `if (x == false)`
- Each attribute on its **own line** above the member
- Named arguments at call site — when adjacent parameters are easy to mix up
- More than two ctor / method parameters — one per line:

```csharp
public FooService(
    IBar bar,
    IBazService bazService,
    IQuxProvider quxProvider)
{
```

## Comments

Production code **without** `//` and `///`, no XML-doc. Exception: generated code, Unity templates, Editor-only — keep the generator's style.

## Types and `var`

**Explicit types**; **`var` is forbidden**.

## Content

- No magic numbers (except obvious `0`); defaults in SerializeField are ok
- **`foreach`** by default; **`for`** when an index, reverse pass or parallel arrays are needed
- Descriptive loop names (`interactableTarget`); `i` only for an index
- No `goto` in new code
- **Prefer `switch`** (statement or expression) over `if` / `else if` chains when other things are equal — including runtime thresholds via `_ when <condition>` arms. Several sequential `if (condition) return X;` → one `switch`, unless the branches differ in shape, not just in return value
- Event invocation: `handler?.Invoke(...)`, not `handler(...)`
- LINQ: short chains; not by default on hot paths ([design.md](design.md))
- Extension methods — rarely; static class `{Type}Extensions`
- Public Api: named type / `record` instead of a long `ValueTuple`

## `sealed`

- **Required:** `sealed class … : ExtendedException`; `sealed class *Config : ScriptableObject`
- **Recommended:** new leaf View / InputHandler
- Service / Model — usually **not** sealed unless leaf

## Async

- **UniTask** / `UniTask<T>` / `UniTaskVoid` only, never `Task`
- `Async` suffix required
- Fire-and-forget: `UniTaskVoid` or `.Forget()`
- `async void` forbidden (except an event handler with `try` / `catch`)
- `CancellationToken cancellationToken` — last parameter; pass it down

## IDisposable

- **Explicit** `void IDisposable.Dispose()` — cleanup only inside the type; **public** `void Dispose()` — when disposed from outside
- Subscription field — `IDisposable` where applicable

## Static

- `static` methods — **only when strictly necessary**: stateless factory, `RuntimeInitializeOnLoad`, extension methods, `static void Register*(IContainerBuilder)` in a LifetimeScope. Private class helpers are instance methods; no static helpers inside service / logic classes
- **Helper classes** — `internal static class {Feature}Helper` (`public` only if another assembly calls it): stateless pure functions, no MonoBehaviour, DOTween, FMOD. View math → `{ViewLayer}/{Feature}/`, domain math without Unity presentation → `{CoreLayer}/{Feature}/`. The `Helper` suffix is **allowed**

## SerializeField and MonoBehaviour

- `[SerializeField] private` named `_camelCase` — every private field, including ScriptableObject Config fields; inline default: `[SerializeField] private float _duration = 0.2f;`
- **Do not reorder** `[SerializeField]` fields (Inspector)
- Public access to serialized data — through properties, not public fields
- Field explanation — `[Tooltip("...")]`, not a comment
- Config SO: `[CreateAssetMenu(menuName = "Configs/...")]`
