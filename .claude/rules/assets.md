---
paths:
  - "ButchersGames/Assets/_Project/Prefabs/**"
  - "ButchersGames/Assets/_Project/Graphics/**"
  - "ButchersGames/Assets/_Project/Configs/**"
---

# Assets, prefabs, scenes

Tools, move / rename via MCP — [unity-mcp.md](unity-mcp.md); UI layout, sprites, fonts — [ui.md](ui.md); code roles — [feature-anatomy.md](../reference/feature-anatomy.md). Here: where an asset lives, its name, how a prefab / scene object is built.

## Folders

```
Assets/_Project/
├── Scenes/                          Bootstrap (boot only), Core (gameplay + all visuals)
├── Prefabs/{Group}/{Category}/      LevelContent/Doors, Levels, Runner, UI, Debug
├── Graphics/{Group}/{Category}/     art of that category; mirrors Prefabs/
│   ├── Common/{Materials|Shaders|Sprites|Textures}/   shared by 2+ categories
│   └── Environment/{Sea|Sky}/, Fonts/, UI/{Screen}/
├── Configs/{Gameplay|LevelContent|Presentation|UI}/   SO instances
└── Settings/                        project-owned settings assets
Assets/Settings/    URP, Volume, Input actions
Assets/ThirdParty/  third-party assets and code
```

- Asset lives next to its only consumer; second category starts using it → `Graphics/Common/{Type}/`
- Category folder is flat; several kinds / parts with own files → subfolder per kind or part (`Doors/Rich/`, `Runner/Animations/`, `Runner/VFX/{Effect}/`); files shared by the category stay at its root (`Doors/Door.mat`)
- Split by type (`Materials/`, `Textures/`, `Shaders/`) — only in `Common/`
- Configs: `Gameplay` — Core settings (`I*Settings`), `LevelContent` — content prefab configs, `Presentation` — View / camera / audio, `UI` — screens
- Nothing new in the `Assets/` root; folder created with its first asset
- `Resources/` — only assets a package loads by name (TMP Settings, DOTween, PerfMeter): everything there ships in the build
- Package-created folders (`TextMesh Pro/`, `Plugins/`) stay where the package put them
- `_Project/Visual/` — temporary dump of employer source assets, deleted before publishing: an asset from it gets used → `AssetDatabase.MoveAsset` (keeps GUID) into `Graphics/…` before the task is done; nothing new there; end of such a task — `AssetDatabase.GetDependencies`: nothing outside `Visual/` depends on it

## Naming

- PascalCase, English, no spaces; `_` only between base and variant / part / state
- Name says what it is, not where it came from: imported names (`atlas_0`, `SM_Veh_*`, `slot_transparent 1`) are renamed on import
- TMP font assets and presets — [ui.md](ui.md)

| Asset | Pattern | Example |
|---|---|---|
| Base prefab | `{Category}` | `Door`, `Obstacle`, `FloatingText` |
| Variant | `{Base}_{Kind}` | `Door_Rich`, `FloatingText_Loss`, `CollectablePreset_Slalom` |
| Level | `Level_{NN}` | `Level_01` |
| Model prefab | `{Object}Model` | `CharacterModel` |
| Config SO | `{Feature}Config` | `RunnerMovementConfig`, `SafeObstacleConfig` |
| Mesh / texture / material | `{Object}[_{Part}]` | `Door_Rich_Frame`, `Gauge_Empty`, `Money` |
| Texture atlas | `{Object}Atlas` | `PropsAtlas`, `RunnerAtlas` |
| Shared material | `{Shader}_Base` | `Toon_Base` |
| Animation clip | `{Object}_{Action}` | `Door_Open`, `Character_Walk` |
| Animator controller | `{Object}` | `Door`, `Runner` |

## Prefabs

- Reused object or level content → prefab; plain scene GameObject only for one-per-scene objects (camera, `LevelLoader`, `CoreEntryPoint`)
- Family sharing structure / components → base prefab + variants; a variant overrides data (text, color, sprite, mesh, material, config), not structure
- Content inside `Level` / `CollectablePreset` — nested prefab instances, never unpacked
- Model prefab (FBX variant: materials, rig, no logic) — next to its FBX in `Graphics/`; the gameplay prefab in `Prefabs/` nests it (`Runner` ← `CharacterModel`)
- Gameplay content structure (only the children it needs, no empty placeholders):

```
{Name}         logic MonoBehaviour, layer Default
├── Model      visual pivot (rotation, animation)
│   └── Mesh   MeshFilter + MeshRenderer only
├── Trigger    trigger collider, layer GameplayTrigger
├── Collider   solid collider
└── Canvas     world-space UI
```

- Child names — role in PascalCase (`LeftLeaf`, `HandStartPoint`, `WealthIndicatorAnchor`); no default names (`GameObject (1)`, `Cube`)
- Physics filtering — layers + collision matrix (`Player`, `GameplayTrigger`, `Track`), not code
- References between prefab parts — serialized fields set in the prefab

## Scenes

- Core roots grouped, groups split by separator objects `===============` (tag `EditorOnly`, nothing references them): visuals → cameras → `CoreEntryPoint` → UI (`EventSystem`, `Canvas`) → gameplay (Performers, `Runner`, `LevelLoader`)
- New root object goes into its group; a scene object that is a prefab stays a prefab instance
- Lighting, environment (ambient, skybox, fog), volumes — Core only, never Bootstrap; Core's environment applies only while Core is the active scene (`SceneManager.SetActiveScene`)

## Adding content

1. Art → `Graphics/{Group}/{Category}/`, renamed to convention
2. Prefab → `Prefabs/{Group}/{Category}/`, variant of the base if the family exists
3. Config → `Configs/{…}/{Feature}Config.asset`
4. Hook up: `Level/Content/{Category}`, config list or scene group
5. No references outside `_Project` except packages / `ThirdParty`; console clean
