---
paths:
  - "ButchersGames/Assets/_Project/Scripts/**/*.cs"
---

# Architecture

Style — [codestyle.md](codestyle.md); classes / methods / variables — [design.md](design.md); exceptions and Guard — [exceptions.md](exceptions.md).

**Read [feature-anatomy.md](../reference/feature-anatomy.md) first** when creating a feature, Model, Service, View, Presenter, UI screen, Performer, Loader, Provider, Registry or InputHandler, or registering one in CoreScope. It holds the feature template, role details, registration rules and MVP / UI-screen structure.

## Decision tree

```
User input? → InputHandler → I*Service (UI screen: View click → Presenter → I*Service)
Gameplay state / rules? → Model + Service (Core)
View reacts to Model? → Presenter (see "Model → View via Presenter")
Display / animation / VFX? → ViewComponents View
Scene data for Core? → Provider (ViewComponents → Api)
DI / bootstrap / scene load? → ProjectScope / CoreScope
```

## Layers and asmdef

```
Assets/_Project/Scripts/
├── Infrastructure/
│   ├── Bootstrap/           Infrastructure.Bootstrap  — ProjectScope, EntryPoint
│   ├── ExtendedExceptions/  ExtendedExceptions        — ExtendedException, Guard
│   ├── Persistence/         Infrastructure.Persistence — PlayerPrefs and other storage adapters
│   └── Audio/               Infrastructure.Audio — FMOD adapters for Core audio ports (IAudioLoader)
├── Core/
│   ├── Loop/Api/            Core — loop ports (ns Core.Loop): IInputTickable, IGameplayTickable, IPresentationTickable
│   ├── Lifecycle/           Core — scope lifecycle (ns Core.Lifecycle): CoreScopeCancellationSource; Api/ — ICoreScopeCancellation, ISubscriptionLifecycle, IWarmupLifecycle
│   ├── Validation/          Core — SessionValidation (walks IValidatable from DI before PrepareGame)
│   ├── Bootstrap/           Bootstrap — CoreScope, CoreEntryPoint, GameLoop; Scene/ — CoreLoader + Api/ ISceneLoader, LoadSceneMode (Core.Bootstrap.Scene)
│   ├── Gameplay/{Feature}/  Core — Model, Service, Api (incl. I*Settings)
│   └── Input/               Core — input ports Api/ (IDragInput, ns Core.Input) and {Feature}/ InputHandler → Service (ns Core.Input.{Feature})
├── Input/                   Input — device adapters: implement Core ports (I*Input, IInputTickable), read Input System
├── UI/{Screen}/             UI — screen View, Presenter, Config (MVP)
├── ViewComponents/{Feature}/ ViewComponents — View, Providers, SO Config, scene MonoBehaviour
└── Debug/                   Debug — Editor-only utilities (#if UNITY_EDITOR), no gameplay references
```

`A ← B` means **B references A**:

```
ExtendedExceptions  ←  Core  ←  ViewComponents
Core  ←  Input  ←  Bootstrap  ←  Infrastructure.Bootstrap
Core  ←  Infrastructure.Persistence  ←  Bootstrap
Core  ←  UI  ←  Bootstrap
Core  ←  Infrastructure.Audio  ←  Infrastructure.Bootstrap
```

| Assembly | References | Contains |
|---|---|---|
| **ExtendedExceptions** | — | `ExtendedException`, `Guard` |
| **Infrastructure.Persistence** | Core | persistence adapters (PlayerPrefs etc.) |
| **Infrastructure.Audio** | Core, UniTask, FMOD | FMOD adapters for Core audio ports (`FmodAudioLoader`) |
| **Core** | ExtendedExceptions, ContentValidation, UniTask, R3 | gameplay, `Api/` ports (no SO Config), loop phases, SessionValidation |
| **Input** | Core, Unity.InputSystem | device adapters: `DragInput` etc. (plain C#, implement Core ports), `ITrigger` |
| **ViewComponents** | Core, ExtendedExceptions, ContentValidation, UniTask, VContainer, FMOD, Splines, R3 | View, Providers, SO Config |
| **UI** | Core, ExtendedExceptions, ContentValidation, DOTween, TMP, uGUI, R3 | UI screens: View, Presenter, Config |
| **Bootstrap** | Core, Input, UI, ViewComponents, Infrastructure.Persistence, ExtendedExceptions, ContentValidation, VContainer, UniTask | CoreScope, CoreEntryPoint, GameLoop |
| **Debug** | Unity.InputSystem | Editor-only debugging |
| **Infrastructure.Bootstrap** | Bootstrap, Core, Infrastructure.Audio, VContainer, UniTask | ProjectScope, Core loading (waits for `IAudioLoader`) |

**`Api/` everywhere.** Interfaces (ports), DTOs and `Exceptions.cs` of any folder live in its `Api/` subfolder — even when the folder holds nothing but interfaces. A folder with no ports (e.g. `GameLoop`) simply has no `Api/`. Namespace has no `.Api` suffix.

VContainer, UniTask, R3 are packages; do not add them to asmdef by hand (except explicit precompiled refs when needed).

## Dependency rule

| Forbidden | Why |
|---|---|
| `Core` → `ViewComponents` | Core does not know the scene |
| `Core` → `Input` | input ports belong to Core (consumer); the device adapter in `Input` depends on Core, not vice versa |
| View calls `I*Service` directly | bypasses the input adapter (InputHandler; for a UI screen — Presenter) |
| Model uses `UnityEngine.*` | state must be pure C# |
| View makes gameplay decisions / changes Model | logic lives only in Service |
| View holds a reference to a concrete `Model` class | a Presenter sits between them, see below |

Allowed: `ViewComponents` → `Core` (implements `I*View` / `I*Performer` / `I*Loader` / `I*Provider`); `Input` → `Core` (implements `I*Input` / `IInputTickable`); Bootstrap registers concrete Views in DI.

## Model → View via Presenter

A View **never** references a concrete `Model` class (own feature or another). If a View must react to a `*Model` change, a **Presenter** (plain C#, not MonoBehaviour) is mandatory: ctor DI on the `*Model`(s) + a narrow passive `I*View` port. The Presenter owns presentation logic (classic MVP): it subscribes via R3, decides what to show and calls `Show` / `Hide` / `SetX(value)` / `Play(slot)`. The View only applies the call. Legitimate Core ports (`I*Service` / `I*Provider`) stay in the View / InputHandler as usual. Details — [feature-anatomy.md](../reference/feature-anatomy.md).

## DI scopes

`VContainer.Unity.LifetimeScope` subclasses:

| Scope | Scene | Configure |
|---|---|---|
| **ProjectScope** | Bootstrap | EntryPoint, scene loader (`ISceneLoader` etc.) |
| **CoreScope** | Core | gameplay features, `CoreEntryPoint` |

Parent binding — Inspector only: `ProjectScope` on the Bootstrap EntryPoint; `CoreScope` Parent Reference → Type = `ProjectScope`; Core loads additively from the Bootstrap EntryPoint. **Forbidden:** `GameLifetimeScope`, `LifetimeScope.EnqueueParent`.

### Entry points

| EntryPoint | Scope | Start | Dispose |
|---|---|---|---|
| Infrastructure Bootstrap EntryPoint | Project | additive load of Core | — |
| `CoreEntryPoint` | Core | `SessionValidation` → `IWarmupLifecycle.Warmup()` → `ISubscriptionLifecycle.Start()` → `PrepareGame()` | `ISubscriptionLifecycle.Stop()` |
| `GameLoop` | Core | — (`ITickable` only) | — |

`RegisterEntryPoint<T>()` is only VContainer's way to get lifecycle callbacks, not an architectural concept. Core has exactly two:

- `CoreEntryPoint` — the **only** starter of Core gameplay: gets `SessionValidation`, `IReadOnlyList<IWarmupLifecycle>`, `IReadOnlyList<ISubscriptionLifecycle>` and `IGameFlowService` via DI; does not list concrete Presenters / services in its ctor
- `GameLoop` — frame tick only: iterates all `IInputTickable` (read input), `IGameplayTickable` (simulation, `Tick(deltaTime)`), `IPresentationTickable` (apply frame result to scene) in that order. No phase classes, no feature startup, no logic. A View for which applying every change is expensive stores values in `Set*` and applies them once in `IPresentationTickable.Tick()`. `IGameplayInputBlock` — gameplay-input block flag; `GameplayInputBlock` derives it from `GameStateModel.State` (blocked everywhere except `Run`), read by InputHandlers

One pattern for all types:

- New loop participant → implement `I*Tickable`, register `.As<I*Tickable>()` — not a separate `ITickable` / `RegisterEntryPoint`
- Long-lived subscriptions (InputHandler, Presenter, observer service with no ctor consumer — e.g. `GameResultDetector`, `WealthPointsModifierService`) → `ISubscriptionLifecycle` (`Start` / `Stop`), `.As<ISubscriptionLifecycle>()`; **no** own `IStartable` / `RegisterEntryPoint` / `RegisterBuildCallback`
- Init after session Validate (pools, dictionaries, …) → `IWarmupLifecycle`, `.As<IWarmupLifecycle>()`

## Soft-checks

Bad data and invalid configuration → `ExtendedException` (usually via `Guard`, see [exceptions.md](exceptions.md)).

**Forbidden:** soft `return` / early exit without throw on broken data; `LogWarning` / `Debug.LogWarning` (and similar) **instead of** an exception; swallowing an error and continuing in Service / Validate / Provider.

UX gates in an InputHandler (e.g. busy → ignore click) are not soft-checks on bad data; they are allowed input behaviour.

## Do not propose

ECS, System Groups, Event Queue, Command Bus, CQRS, a Presenter for every gameplay entity that already talks through event ports (`I*Provider` / `ITriggerReaction` etc.) rather than a `Model` (Model / Service / View is the default for those: pickups, obstacles, `WealthPointsModifierCollider`, `Obstacle`). A Presenter is mandatory only where the View would otherwise reference a concrete `Model` class.
