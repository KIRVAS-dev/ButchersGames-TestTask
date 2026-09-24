---
paths:
  - "ButchersGames/Assets/_Project/Scripts/**/*.cs"
---

# Design: classes, methods, variables

General practices (SOLID, DRY / YAGNI / KISS, Code Complete) are not restated. Here — where the project picks a specific option. Layers and roles — [architecture.md](architecture.md); format and naming — [codestyle.md](codestyle.md); errors — [exceptions.md](exceptions.md).

- **YAGNI:** no type, layer, parameter or flexibility "for the future" without a second consumer
- **Hot path** (Update and frequently called code): no scene search, repeated `GetComponent`, extra allocations, LINQ. `AggressiveInlining` — only on a proven hot path
- **Inheritance** — only for an "is-a" relation + Liskov; otherwise composition. Never inheritance just to reuse code

## Assembly visibility (`internal` vs `public`)

Default `internal`. `public` only when another assembly (`asmdef`) actually names the type or member: call, field, base type, implementation, `new`, DI `Register<T>` / `RegisterEntryPoint<T>`.

Keep `public` even if the implementation is "inside the feature" for:

- a concrete type registered by another assembly (Bootstrap → Model, Service, View, Presenter)
- an `Api/` port (`I*Service`, `I*View`, `I*Provider`, DTO, exception) implemented or called by another assembly; without external references — `internal`
- a member implicitly implementing a public interface, and `override` — access matches the contract
- a ctor called by another assembly
- a public field of `MonoBehaviour` / `ScriptableObject`: Unity serializes public fields and `[SerializeField]`; narrowing to `internal` or `private` without `[SerializeField]` wipes Inspector data. If nothing outside the assembly reads it — `[SerializeField] private`, not bare `internal`

Do not add `InternalsVisibleTo` to expose `internal`. Members of an `internal` type are invisible outside anyway — in new code they are `internal` too, except interface implementations and serialized fields.

## Explicit vs implicit interface implementation

If a member can be **explicit** (`IFoo.Bar`) at no cost — **prefer explicit**: consumers already go through the port, no casts inside the class.

- **Events** — always implicit (`public event …`): explicit needs `add` / `remove` + backing field for no gain
- **`IValidatable`** — see [exceptions.md](exceptions.md) (owned config SO — `public void Validate()`, View / MB — explicit)
- Also implicit when explicit hurts: calls through the concrete type, Unity / serialization, noticeably clumsier code

## Types

- No public state fields (except DTO / `record` / readonly configs)
- Data-only class or `record` → DTO in `Api/`; minimal `CopyWith*` is fine; rules live in Service
- Stateless verb class (`*Initializer`, process-only) → a method of another type or a static Helper
- Managed OOP (Model, Service, View, Bootstrap): `class` / `record` by default; no `struct` "for perf" without need
- A method must strengthen the type's abstraction; if it blurs it — another type or layer

## Methods

- **Name:** `void` — verb + object (`PlayOpenAnimation`, not `HandleOutput` / `ProcessData`); returning — meaning of the result (`RemainingCharges`, `IsReady`). No side effect hidden from the name
- **Order in file:** top-level method above, the helpers it calls below (inside the Methods block from codestyle.md)
- **Parameters:** ~≤7, more → `record` / options; read-only input collection — `IReadOnlyList` / `IReadOnlyCollection` / `IEnumerable`; never mutate parameters as working variables; results via `return` / `TryX`, not `out` / `ref` chains; `out` after the others, `CancellationToken` still last; non-obvious `bool` argument → `enum` or two honestly named methods; non-trivial lambda → named method
- **Command vs query:** invalid command at the boundary → typed exception ([exceptions.md](exceptions.md)); query — `bool` / `TryX`, no throw on a UX gate
- **Control flow:** guards and early exit at the top; nesting ≤ ~3–4; many parallel `if` / `case` → table (`Dictionary` / config SO) or polymorphism. Method length alone is not a problem — a blurred purpose or deep nesting is

## Variables

- Declare and initialize next to first use; minimal scope (local → private field → wider only when needed)
- One variable — one role; no sentinel values ("counter, but −1 = error") → `bool` / nullable / `Try` / typed exception
- Complex boolean condition → local `bool` named after its meaning (`isReadyToSave`)
- Values: `const` / `static readonly` → Config SO / Provider → DI; no bare literals, no runtime config "just in case"
- Dependencies through DI / ports, not static / mutable shared state
- Inspector, hierarchy, references — edit in the Unity Editor through Coplay / Pipeline ([unity-mcp.md](unity-mcp.md))
