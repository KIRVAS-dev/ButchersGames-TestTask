# Feature anatomy

Reference for [architecture.md](../rules/architecture.md): read before creating a feature or any of its roles. Layers, dependency rule, DI scopes and lifecycle patterns stay in architecture.md.

## Feature template `{Feature}`

New features repeat this anatomy.

```
Core/Gameplay/{Feature}/
├── Api/
│   ├── I{Feature}Service.cs
│   ├── I{Feature}View.cs          — port; implementations in ViewComponents
│   ├── I{Feature}Provider.cs      — optional
│   ├── I{Feature}Settings.cs      — pure C# settings port (if a Config is needed)
│   ├── {Feature}*Data.cs          — DTO
│   └── Exceptions.cs
├── {Feature}Model.cs
├── {Feature}Service.cs            — depends on I{Feature}Settings, not on the SO
└── {Feature}*Registry.cs          — optional

Core/Input/{Feature}/
└── {Feature}InputHandler.cs

ViewComponents/{Feature}/
├── Api/
│   ├── {Feature}Config.cs         — ScriptableObject : I{Feature}Settings (if needed)
│   └── Exceptions.cs              — view / Inspector errors
├── {Feature}View.cs               — : I{Feature}View
└── {Feature}*Provider.cs          — : I{Feature}Provider (if needed)
```

## Roles

| Role | Where | Responsibilities |
|---|---|---|
| **Model** | `Core/Gameplay/{Feature}/` | Mutable state. No side effects. No `UnityEngine.*` |
| **Service** | `Core/Gameplay/{Feature}/` | All feature business logic: validation, orchestration, `ExtendedException`. The only external entry point (`I{Feature}Service`) |
| **Settings** | `Core/Gameplay/{Feature}/Api/` | Pure C# port `I{Feature}Settings` — getters, no Unity |
| **Config** | `ViewComponents/{Feature}/Api/` | `[CreateAssetMenu]` ScriptableObject implementing `I{Feature}Settings`. `Validate()` lives here |
| **Registry** | `Core/Gameplay/{Feature}/` | Index / lookup over Provider data |
| **Api/** | `Core/Gameplay/{Feature}/Api/` | Interfaces, DTOs, `Exceptions.cs` |
| **View** | `ViewComponents/{Feature}/` | Displays **one concrete entity or screen** of the scene (character, level, HUD), implements `I{Feature}View`. DOTween, VFX, Animator. Does not change game state. `IValidatable.Validate()` via Guard ([exceptions.md](../rules/exceptions.md)) |
| **Performer** | `ViewComponents/{Feature}/` | One per scene, no entity of its own: takes a semantic event type (`enum`) and plays effects from a record table (FMOD sound, VFX). Implements `I{Feature}Performer` from Core Api. Does not change game state. Example: `FeedbackPerformer` |
| **Loader** | `ViewComponents/{Feature}/` | One per scene: on a Core command (`Load{X}(index)`) spawns a content instance (prefab from its config), keeps it as current and reports readiness via `{X}Loaded`; gives Core the count and settings of the loadable list. Implements `I{Feature}Loader` from Core Api. Does not change game state. Hands the loaded instance's content to a persistent plain C# `Current{X}` (DI Singleton) that stores it and implements `I*Registry` / `I*Provider` ports for Core; readers depend on `Current{X}`, not on the Loader. Example: `LevelLoader` + `CurrentLevel` |
| **Provider** | `ViewComponents/{Feature}/` | Scene → DTO / data for Core through a port. Config errors → view exceptions |
| **InputHandler** | `Core/Input/{Feature}/` | Thin adapter: Core input port (`I*Input`) → `I{Feature}Service`. No references to ViewComponents or Input. Guard only for UX gates (busy etc.) |
| **Input adapter** | `Input/` | Plain C# device adapter: implements a Core port (`I*Input`, `IInputTickable`), reads Input System. `Register<T>(Singleton).As<…>()` |
| **Scope** | `Core/Bootstrap/CoreScope` | `Register{Feature}(builder)` |

## Data flow

```
[low-level Input] → {Feature}InputHandler → I{Feature}Service (Service)
  → Model / Registry  ←  I{Feature}Provider (implemented in View)
  → Presenter (R3 subscription to Model) → I{Feature}View (View: animation / VFX)
```

The Presenter holds the R3 subscription to the `Model`, not the View; business rules stay in Service.

## CoreScope registration

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

- **Config** (SerializeField on CoreScope, type from ViewComponents) → `RegisterInstance<I*Settings>(so)`; Provider → `.As<I*Provider>()`
- **Scene View** → `RegisterComponentInHierarchy<TView>().As<I*View>()`
- **Model, Service, Registry, InputHandler** → `Register<T>(Lifetime.Singleton)`; Service → `.As<I*Service>()`
- Core **never** references a concrete Config SO — only `I*Settings`

## Adding a feature

1. `Core/Gameplay/{Feature}/Api/` — `I{Feature}Service`, View / Provider / Settings ports, DTOs, `Exceptions.cs`
2. `Core/Gameplay/{Feature}/` — Model, Service, Registry if needed
3. `ViewComponents/{Feature}/` — View, Providers, `{Feature}Config` SO; `Api/Exceptions.cs` for view errors
4. `Core/Input/{Feature}/` — InputHandler, if there is low-level input without a screen (drag, keys). UI-screen input (button clicks) goes through the Presenter, see **UI screens**
5. `CoreScope` — `Register{Feature}`, SerializeFields for config / providers; warmup `.As<IWarmupLifecycle>()`, subscriptions `.As<ISubscriptionLifecycle>()`
6. Core scene — View, Providers; references on CoreScope

Check on Core: happy path + edge cases (busy, cancel, invalid data → exception).

## Presenter — classic MVP

Not limited to UI screens: the same pattern applies to any View that has no Model of its own but reacts to another feature's (e.g. `CharacterAppearanceView` reacts to `WealthMeterModel.Stage` via `CharacterAppearancePresenter`; `RunnerTrackFollower` to `RunnerMovementModel.LateralOffset` / `State` via `RunnerMovementPresenter`). Only what would otherwise be a direct `Model` reference moves into the Presenter.

The Presenter owns **presentation logic**: subscribes to Core state (one or more `*Model`), decides what the View shows or plays, and hands it a ready command (`Show` / `Hide` / `SetX(value)` / `Play(slot)`).

| Who | Does | Does not |
|---|---|---|
| **Presenter** | Maps Core state to a View command: `switch` on `GameState`, folds several `*Model` into one result, picks slot / variant, formats values | Gameplay decisions: does not change `Model`, validate, or decide the game outcome — that is Service. Core reports **what happened** (`Win`, `Stopped`), Presenter decides **what to show** |
| **View** | Applies the command | Does not read Core, subscribe, or choose what to show |

- If the Presenter would have to guess a cause from indirect signs, add the missing fact to Core as an event / port (e.g. `IObstacle.Hit`) — do not invent a new state value for display (`RunnerMovementState` stays `Moving` / `Stopped`)
- One Presenter may fold several `*Model` into one View command
- Examples: `CharacterAnimationPresenter` (`GameStateModel` + `RunnerMovementModel` → `CharacterAnimationSlot` → `ICharacterAnimationView.Play`), `FeedbackPresenter` (`GameState` → `FeedbackType`), `CharacterAppearancePresenter` (`WealthMeterModel.Stage` → `SetActiveStage`)
- Registration: `Register<TPresenter>(Lifetime.Singleton).As<ISubscriptionLifecycle>()` in `CoreScope`, no `I*Service` of its own in Core Api. `ISubscriptionLifecycle` gives eager resolution via the list in `CoreEntryPoint`

## UI screens (GameUI, MVP)

A UI screen (start / HUD / result etc.) is a special case of the Presenter pattern: no gameplay model of its own, only display of existing Core state (`GameStateModel`, `WealthMeterModel`, …). Screens get a fixed file structure.

```
UI/{Screen}/
├── Api/
│   ├── I{Screen}View.cs        — passive contract: Show() / Hide() / SetX(value) + `event Action {Action}Clicked` for buttons; no R3, no conditions
│   └── Exceptions.cs           — view / Inspector errors, as for a regular View
├── {Screen}View.cs             — : MonoBehaviour, I{Screen}View — only applies calls, decides nothing
└── {Screen}Presenter.cs        — plain C# (not MonoBehaviour)
```

| Role | Responsibilities |
|---|---|
| **Presenter** | ctor DI on an existing Core `*Model` / `I*Service` (no new model for the screen) + `I{Screen}View`. Translates Core state into `Show` / `Hide` / `SetX` via R3; subscribes to the View's `{Action}Clicked` and calls `I*Service` — the screen's only input point |
| **View** | Implements `I{Screen}View`. `SetActive` / `Set*` on UI elements; forwards uGUI `Button` clicks as `event Action` — no Core reads, no R3, no conditions, no Service calls |

**Screen input:** `Button.onClick` → View raises `{Action}Clicked` → Presenter calls `I*Service` (e.g. `IGameFlowService.StartGame()`). No separate InputHandler, no `Core/Input/Api` port, no `ITrigger` wrapper for screen buttons. The Presenter only forwards intent and makes **no gameplay decisions** — validation (`Guard`, state transitions) stays in Service. `InputHandler` remains for low-level input without a screen (drag, keys).

**Do not replicate** the `UI/{Screen}/` structure (Presenter + `I{Screen}View` with screen naming) for gameplay entities that talk through event ports rather than a `Model` (pickups, obstacles — `WealthPointsModifierCollider`, `Obstacle`): those keep the plain Model + Service (Core) / View (ViewComponents) pair from the template.
