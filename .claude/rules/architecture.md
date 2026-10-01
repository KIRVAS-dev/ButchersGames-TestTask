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
│   ├── ExtendedExceptions/  ExtendedExceptions        — ExtendedException, Guard (ns Infrastructure.ExtendedExceptions)
│   ├── Persistence/         Infrastructure.Persistence — PlayerPrefs and other storage adapters
│   └── Audio/               Infrastructure.Audio — FMOD adapters; Api/ — IAudioLoader (consumed by Infrastructure.Bootstrap)
├── Core/
│   ├── Loop/Api/            Core — loop ports (ns Core.Loop): IInputTickable, IGameplayTickable, IPresentationTickable
│   ├── Lifecycle/           Core — scope lifecycle (ns Core.Lifecycle): CoreScopeCancellationSource; Api/ — ICoreScopeCancellation, ISubscriptionLifecycle, IWarmupLifecycle
│   ├── Loading/             Core — app loading state (ns Core.Loading, registered in ProjectScope): LoadingModel, LoadingService; Api/ — IReadOnlyLoadingModel, ILoadingService
│   ├── Bootstrap/           Bootstrap — CoreScope, CoreEntryPoint, GameLoop, ScopeLifecycle; Scene/ — CoreLoader + Api/ ISceneLoader, LoadSceneMode (Core.Bootstrap.Scene)
│   ├── Gameplay/{Feature}/  Core — Model, Service, Api (incl. I*Settings)
│   └── Input/               Core — input ports Api/ (IDragInput, ns Core.Input) and {Feature}/ InputHandler → Service (ns Core.Input.{Feature})
├── Input/                   Input — device adapters: implement Core ports (I*Input, IInputTickable), read Input System
├── UI/{Screen}/             UI — screen View, Presenter, Config (MVP)
├── ViewComponents/{Feature}/ ViewComponents — View, Providers, SO Config, scene MonoBehaviour
└── Debug/                   Debug — Editor-only utilities (#if UNITY_EDITOR, ns ProjectDebug); no assembly references Debug
```

`A ← B` means **B references A**:

```
ExtendedExceptions  ←  Core  ←  ViewComponents
Core  ←  Input  ←  Bootstrap  ←  Infrastructure.Bootstrap
Core  ←  Infrastructure.Persistence  ←  Bootstrap
Core  ←  UI  ←  Bootstrap
Core  ←  Infrastructure.Audio  ←  Infrastructure.Bootstrap
ViewComponents  ←  Infrastructure.Bootstrap
UI  ←  Infrastructure.Bootstrap
Core  ←  Debug
```

| Assembly | References | Contains |
|---|---|---|
| **ExtendedExceptions** | — | `ExtendedException`, `Guard` |
| **Infrastructure.Persistence** | Core | persistence adapters (PlayerPrefs etc.) |
| **Infrastructure.Audio** | Core, UniTask, FMOD | `IAudioLoader` + `FmodAudioLoader` |
| **Core** | ExtendedExceptions, ContentValidation, UniTask, R3; `noEngineReferences` | gameplay, `Api/` ports (no SO Config), loop phases, loading state |
| **Input** | Core, Unity.InputSystem | device adapters: `DragInput` etc. (plain C#, implement Core ports), `ITrigger` |
| **ViewComponents** | Core, ExtendedExceptions, ContentValidation, UniTask, VContainer, FMOD, Unity.InputSystem, Splines, Cinemachine, DOTween, R3 | View, Providers, SO Config |
| **UI** | Core, ExtendedExceptions, ContentValidation, VContainer, DOTween, TMP, uGUI, R3 | UI screens: View, Presenter, Config |
| **Bootstrap** | Core, Input, UI, ViewComponents, Infrastructure.Persistence, ExtendedExceptions, ContentValidation, VContainer, UniTask | CoreScope, CoreEntryPoint, GameLoop, ScopeLifecycle |
| **Debug** | Core, Unity.InputSystem, VContainer | Editor-only debugging |
| **Infrastructure.Bootstrap** | Bootstrap, Core, Infrastructure.Audio, ViewComponents, UI, ExtendedExceptions, ContentValidation, VContainer, UniTask | ProjectScope, Core loading (waits for `IAudioLoader`), loading screen registration |

**`Api/` everywhere.** Interfaces (ports), DTOs and `Exceptions.cs` of any folder live in its `Api/` subfolder — even when the folder holds nothing but interfaces. A folder with no ports (e.g. `GameLoop`) simply has no `Api/`. Namespace has no `.Api` suffix.

R3 is a precompiled NuGet DLL (`Assets/Packages/`): listed in `precompiledReferences` where `overrideReferences` is on (Core, ViewComponents, UI), otherwise auto-referenced.

## Dependency rule

| Forbidden | Why |
|---|---|
| `Core` → `ViewComponents` | Core does not know the scene |
| `Core` → `Input` | input ports belong to Core (consumer); the device adapter in `Input` depends on Core, not vice versa |
| View calls `I*Service` directly | bypasses the input adapter (InputHandler; for a UI screen — Presenter) |
| Core uses `UnityEngine.*` | Core is pure C#; `noEngineReferences` in `Core.asmdef` makes it a compile error |
| View makes gameplay decisions / changes Model | logic lives only in Service |
| View holds a reference to a concrete `Model` class | a Presenter sits between them, see below |
| Code outside the owning feature takes a concrete `*Model` | write access leaks; use `IReadOnly{Feature}Model` from the owner's `Api/` |
| A consumer takes a port with commands it never calls | command access leaks; listeners take `I*Events`, readers — `IReadOnly*Model` or a query port |

Allowed: `ViewComponents` → `Core` (implements `I*View` / `I*Performer` / `I*Loader` / `I*Provider` / `I{Entity}{Capability}`); `Input` → `Core` (implements `I*Input` / `IInputTickable`); Bootstrap registers concrete Views in DI.

## Model → View via Presenter

A View **never** references a concrete `Model` class (own feature or another). If a View must react to a `*Model` change, a **Presenter** (plain C#, not MonoBehaviour) is mandatory: ctor DI on the `IReadOnly{Feature}Model`(s) + a narrow passive `I*View` port. The Presenter owns presentation logic (classic MVP): it subscribes via R3, decides what to show and calls `Show` / `Hide` / `SetX(value)` / `Play(slot)`. The View only applies the call. A View (`I*View`) is passive: no `*Model`, no `I*Service`, no Core reads. A non-Core dependency the View needs (scene / UI service, e.g. `IRunnerOverlayTarget`) → `[Inject] private void Construct(...)` on the View; the Presenter does not forward it, `I*View` has no setter for it. Other ViewComponents roles (Provider, Performer, Loader, tickers) implement or read Core ports as usual. Details — [feature-anatomy.md](../reference/feature-anatomy.md).

**Read-only model ports:** a `*Model` is mutated only inside its owning feature (Service / simulator / state machine), which takes the concrete class. Everyone else — Presenters, another feature's Service or detector — takes `IReadOnly{Feature}Model` from the owner's `Api/`: `ReadOnlyReactiveProperty<T>` getters, implemented explicitly by the Model (`ReactiveProperty<T>` already is a `ReadOnlyReactiveProperty<T>`). Registration: `Register<TModel>(Lifetime.Singleton).AsSelf().As<IReadOnly{Feature}Model>()`. A model read only by its owner needs no port.

**Ports by access:** a consumer gets only what it uses. Commands — `I{Feature}Service` (InputHandler, another feature's Service, a UI screen's clicks). Facts — `I{Feature}Events`, implemented by the Service / Loader that raises them (e.g. `IWealthMeterEvents`, `ILevelLoaderEvents`). State — `IReadOnly{Feature}Model`. A derived value that is not model state — a narrow query port (e.g. `ILevelProgress.CurrentLevelNumber`). No duplicate reads: a port does not re-expose model state (`IGameStateMachine` has no `State`; read `IReadOnlyGameStateModel`). Registration: one implementation, several `.As<…>()`.

## DI scopes

`VContainer.Unity.LifetimeScope` subclasses:

| Scope | Scene | Configure |
|---|---|---|
| **ProjectScope** | Bootstrap | EntryPoint, scene loader (`ISceneLoader` etc.), loading state, loading screen |
| **CoreScope** | Core | gameplay features, `CoreEntryPoint` |

Parent binding — Inspector only: `ProjectScope` on the Bootstrap EntryPoint; `CoreScope` Parent Reference → Type = `ProjectScope`; Core loads additively from the Bootstrap EntryPoint. **Forbidden:** `GameLifetimeScope`, `LifetimeScope.EnqueueParent`.

### Entry points

| EntryPoint | Scope | Start | Dispose |
|---|---|---|---|
| Infrastructure Bootstrap EntryPoint | Project | `ScopeLifecycle.Start()` → additive load of Core | `ScopeLifecycle.Stop()` |
| `CoreEntryPoint` | Core | `ScopeLifecycle.Start()` → `PrepareGame()` → `ILoadingService.Complete()` | `ScopeLifecycle.Stop()` |
| `GameLoop` | Core | — (`ITickable` only) | — |

`RegisterEntryPoint<T>()` is only VContainer's way to get lifecycle callbacks, not an architectural concept. Core has exactly two:

- `CoreEntryPoint` — the **only** starter of Core gameplay: gets `ScopeLifecycle`, `IGameFlowService` and `ILoadingService` via DI; does not list concrete Presenters / services in its ctor
- `GameLoop` — frame tick only: iterates all `IInputTickable` (read input), `IGameplayTickable` (simulation, `Tick(deltaTime)`), `IPresentationTickable` (apply frame result to scene) in that order. No phase classes, no feature startup, no logic

A View for which applying every change is expensive stores values in `Set*` and applies them once in `IPresentationTickable.Tick()`. `IGameplayInputBlock` — gameplay-input block flag; `GameplayInputBlock` derives it from `IReadOnlyGameStateModel.State` (blocked everywhere except `Run`), read by InputHandlers.

`ScopeLifecycle` — Singleton in **each** scope; `Start()`: `IValidatable.Validate()` → `IWarmupLifecycle.Warmup()` → `ISubscriptionLifecycle.Start()`, `Stop()`: `ISubscriptionLifecycle.Stop()`. Takes only its own scope's registrations via `ContainerLocal<IReadOnlyList<T>>`: VContainer merges parent registrations into a child's `IReadOnlyList<T>`. Never inject these lists directly. `ContainerLocal` drops only parent Singletons → lifecycle participants in ProjectScope are Singleton only.

One pattern for all types:

- New loop participant → implement `I*Tickable`, register `.As<I*Tickable>()` — not a separate `ITickable` / `RegisterEntryPoint`
- Long-lived subscriptions (InputHandler, Presenter, observer service with no ctor consumer — e.g. `GameResultDetector`, `WealthPointsModifierService`) → `ISubscriptionLifecycle` (`Start` / `Stop`), `.As<ISubscriptionLifecycle>()`; **no** own `IStartable` / `RegisterEntryPoint` / `RegisterBuildCallback`
- Init after session Validate (pools, dictionaries, …) → `IWarmupLifecycle`, `.As<IWarmupLifecycle>()`

## Soft-checks

Bad data and invalid configuration → `ExtendedException` (usually via `Guard`, see [exceptions.md](exceptions.md)).

**Forbidden:** soft `return` / early exit without throw on broken data; `LogWarning` / `Debug.LogWarning` (and similar) **instead of** an exception; swallowing an error and continuing in Service / Validate / Provider.

UX gates in an InputHandler (e.g. busy → ignore click) are not soft-checks on bad data; they are allowed input behaviour.

Persisted player data is not authored content: the owning Service normalizes out-of-range values instead of throwing (e.g. `LevelService` restores a saved level index after levels were removed).

## Do not propose

ECS, System Groups, Event Queue, Command Bus, CQRS, a Presenter for every gameplay entity that already talks through event ports (`I*Provider` / `ITriggerReaction` etc.) rather than a `Model` (Model / Service / View is the default for those: pickups, obstacles, `WealthPointsModifierCollider`, `Obstacle`).
