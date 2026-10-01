# Rule candidates

Journal for skill `retro`. Not loaded automatically. A candidate reaching 3 confirmations is offered for moving into its target file, then removed from here.

## Entry format

```
### <short title>
- Rule: <what to do>
- Target: <rules file / CLAUDE.md / skill>
- Why: <problem it prevents>
- Example: <project case>
- Count: <n>
- Episodes: <YYYY-MM-DD> — <session fact>; …
```

## Candidates

### manage_asset move false failure
- Rule: `manage_asset move` returned an error → check the files on disk and the `.meta` GUID before retrying; the move may have succeeded
- Target: unity-mcp.md
- Why: a retry of a completed move can fail or duplicate work
- Example: moving `Core/Audio` files — 4 × "MoveAsset call failed unexpectedly", all files moved with GUIDs kept
- Count: 2
- Episodes: 2026-09-27 — Core/Audio move out of Core; 2026-09-30 — `manage_asset rename` of `RendererVisibilitySwitch.cs` reported failure, file renamed with GUID kept

### runInBackground outside Play Mode
- Rule: set `Application.runInBackground` only in Play Mode; outside it the setter is `PlayerSettings.runInBackground` and may be saved to `ProjectSettings.asset`
- Target: unity-mcp.md
- Why: the WebGL build would stop pausing in an inactive tab
- Example: eval in Edit Mode → `runInBackground: 1` written to `ProjectSettings.asset`
- Count: 1
- Episodes: 2026-09-27 — user asked to enable it during the Core/Audio move

### Infrastructure only from composition roots
- Rule: `Infrastructure.*` is referenced only by composition roots (`Bootstrap`, `Infrastructure.Bootstrap`); a port lives with its consumer
- Target: architecture.md, Dependency rule
- Why: outer layers (ViewComponents, Infrastructure) must not depend on each other
- Example: proposed `ViewComponents → Infrastructure.Audio` for `IStudioListenerAnchor`, withdrawn after the user's question; anchor moved to `ViewComponents/Audio`
- Count: 1
- Episodes: 2026-09-27 — Core/Audio move out of Core

### Internal project types in eval
- Rule: `unity command eval` code cannot see `internal` types of project assemblies → go through a public port (`I*View`) or resolve the type by name via reflection
- Target: unity-mcp.md, Pipeline and arbitrary C#
- Why: compile errors and extra round trips
- Example: eval using `TransformRotator` / `Collectable` → "inaccessible due to its protection level"
- Count: 1
- Episodes: 2026-09-28 — draw call optimization, rotator benchmark and prefab inspection

### FMOD banks match the build target
- Rule: before a player build, verify that `Assets/StreamingAssets/*.bank` match `FMOD/Build/<BuildDirectory>` of the target platform (FMOD Settings → platform → Build Directory); mismatch → FMOD → Refresh Banks, verify again
- Target: CLAUDE.md (build workflow) or the WebGL smoke-test skill once it exists
- Why: Play Mode loads banks from `FMOD/Build/…` directly, so "works in the Editor" says nothing about the banks shipped in the build
- Example: BGMusic relinked and banks rebuilt after the WebGL build → `docs/StreamingAssets/Music.bank` had a silent music event while the Editor played it
- Count: 1
- Episodes: 2026-09-30 — WebGL Portrait template and sound fixes

### Setting history before changing it
- Rule: before changing a value in project settings, a URP asset or a config, run `git log -S<field>` on its file and read why the current value was chosen
- Target: CLAUDE.md, Scope
- Why: a deliberate value gets reverted and the fixed bug comes back
- Example: `WebGL_RPAsset` `m_Cascade2Split` 0.15 → 0.3 brought back the runner hair shadow shimmer; commit `4a95d782c` explained 0.15
- Count: 1
- Episodes: 2026-09-30 — shadow sharpness tuning

### URP asset by quality level
- Rule: resolve the URP asset to edit via `QualitySettings.GetRenderPipelineAssetAt(index)` or its explicit path, not `GraphicsSettings.currentRenderPipeline` (depends on the Editor's active quality level)
- Target: unity-mcp.md
- Why: the wrong platform asset gets edited
- Example: meant `WebGL_RPAsset`, `currentRenderPipeline` returned `PC_RPAsset` after the quality level changed; PC asset edited and reverted
- Count: 1
- Episodes: 2026-09-30 — cascade split change

### Checks after Rider refactorings
- Rule: after `rename_refactoring` of a `[SerializeField]` field remove the auto-inserted `[FormerlySerializedAs]`; after renaming a MonoBehaviour rename its file via `AssetDatabase.RenameAsset` (GUID kept); after `safe_delete` check the file on disk
- Target: rider-mcp.md
- Why: forbidden attribute slips in; class / file name mismatch breaks the MonoBehaviour; "applied" delete leaves files
- Example: `_target` → `_behaviour` got `[FormerlySerializedAs("_target")]`; class rename left `RendererVisibilitySwitch.cs`; `safe_delete` left 2 intact files and 1 empty
- Count: 1
- Episodes: 2026-09-30 — rotator visibility refactoring

### Scene view off for visibility-dependent measurements
- Rule: before measuring anything that depends on `Renderer.isVisible` / `OnBecameVisible` in the Editor, ask the user to close or hide the Scene view; check `SceneView` count = 0
- Target: skills/perfmeter
- Why: the Scene view camera counts as visible and skews the result
- Example: rotator gating A/B — 248 "visible" instead of 32 with the Scene view open
- Count: 1
- Episodes: 2026-09-30 — rotator gating measurements (twice)
