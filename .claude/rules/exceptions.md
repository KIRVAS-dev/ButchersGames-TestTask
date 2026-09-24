---
paths:
  - "ButchersGames/Assets/_Project/Scripts/**/*.cs"
---

# Typed exceptions and Guard

Infrastructure: `ExtendedException`, `Guard`, namespace `ExtendedExceptions`.

## Base

```csharp
public class ExtendedException : Exception
{
    protected ExtendedException(string id, string message)
        : base(message: $"{id}: {message}") { }
}
```

- Console message: `{id}: {text}`
- **id** — kebab-case, feature prefix + sequence number from 1 within the file (`feature-1`, `feature-2`)
- Message **without** a trailing period
- `base(id, message)` — one line in the ctor

## Organization

- One `Exceptions.cs` per feature (usually `{Layer}/{Feature}/Api/`); namespace of the feature **without** `.Api`
- `sealed class … : ExtendedException` — `public` if created or caught in another assembly, else `internal` ([design.md](design.md))
- **No** separate file per type
- **Forbidden:** a generic exception with an arbitrary `reason` string

## View / Inspector

MonoBehaviour, SerializeField, scene.

| Case | Type | Ctor |
|---|---|---|
| Field not assigned | `Missing{Feature}FieldException` | `fieldName`, `objectName` |
| Invalid number on an MB | `Invalid{Feature}ValueException` | `fieldName`, `objectName`, `value` |

- Types with authored wiring / config implement `IValidatable` (`ContentValidation` package); `Validate()` only checks, no side effects
- **When called:** session — `SessionValidation` from DI before `PrepareGame`; level content — after `LoadLevel` / `CurrentLevel.Set`; Editor — `Tools/ContentValidation` (collect-all)
- Init that needs already validated fields (pools, dictionaries, `TurnAnimator` etc.) — `IWarmupLifecycle.Warmup()` after session in `CoreEntryPoint`, not `Awake → Validate`
- View / MB: prefer explicit `void IValidatable.Validate()`; `public void Validate()` only if a call through the concrete type is needed
- References: `Guard.AgainstNull` / `AgainstNullOrEmpty` + typed factory
- Numbers: `Guard.AgainstNegative` / `AgainstNonPositive` / `AgainstLessThan` / … → `Invalid{Feature}ValueException`
- A View message may use `gameObject.name` as `objectName`

## Core / domain

Model, Service, ScriptableObject config without scene binding.

| Case | Type | Ctor |
|---|---|---|
| Invalid value in SO config | `Invalid{Feature}ValueException` | `fieldName`, `value` — **no** `objectName` |
| Domain / context | `Missing/Invalid{Feature}{Topic}Exception` | per feature |

- Config (owned SO): `IValidatable` via **`public void Validate()`** (not explicit) — the owner calls `_config.Validate()` after `Guard.AgainstNull`; session / Editor also go through `IValidatable`. Explicit on a config only adds a `((IValidatable)_config)` cast
- Soft-check policy (no soft return / LogWarning instead of throw) — [architecture.md](architecture.md)

## Guard

- API: `Guard.Against*(…, Func<ExtendedException> exceptionFactory)`; the factory is lazy — the exception is created only on violation
- Checks go through `Guard`, not inline `if` + `throw` with raw text
- Typical methods: `AgainstNull` (`object` and `UnityEngine.Object`), `AgainstNullOrEmpty`, `AgainstNegative`, `AgainstNonPositive`, `AgainstLessThan`, `AgainstGreaterThan`, `AgainstInvalidRange`, `AgainstTrue`

## Local factory (call site)

**≥2** checks with **one** exception type and the same argument shape → move the factory into a **local function** (not a `Func<…>` variable):

```csharp
Guard.AgainstNull(_a, () => Missing(nameof(_a)));
Guard.AgainstNull(_b, () => Missing(nameof(_b)));

return;

ExtendedException Missing(string fieldName) => new MissingFooFieldException(fieldName, gameObject.name);
```

- Local function at the **end** of the method, after an explicit `return;`; PascalCase name (`Missing`, `Invalid`) — required by Rider inspections ("Use local function", "Local functions" naming, "Separate local function with explicit return")
- No `Func<string, ExtendedException> missing = …`
- Single `Guard` — inline `() => new …`, no local function
- Different types in one method — a local function per type, or inline
- `nameof` is mandatory; `CallerArgumentExpression` is out of scope
