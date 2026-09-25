---
paths:
  - "**/*.unity"
  - "**/*.prefab"
---

# Unity MCP

Auto-loads only for `.unity` / `.prefab`. Any scene, prefab, component, asset or project-settings work — read this file first even if no such file was read. C# post-edit sequence (format, lint, compile, console) — [rider-mcp.md](rider-mcp.md).

Project: `ButchersGames/`. Scripts: `Assets/_Project/Scripts/`. Scenes: `Assets/_Project/Scenes/` (Bootstrap, Core). Paths relative to `Assets/` (forward slashes) unless a tool needs absolute.

## Tools and when to use which

- **Coplay MCP** (MCP for Unity, package `com.coplaydev.unity-mcp`, server `unityMCP`, tools `mcp__unityMCP__*`). Group `core` is enabled; other groups (`testing`, `scripting_ext`, `profiling`, `docs`, `ui`, `vfx`, `animation`, …) are off — do not enable without need
- **Unity Pipeline** (official Unity plugin: `unity command <name>` via Bash, package `com.unity.pipeline`). Command list — `unity command --query <word> --detail compact`; parameters of an unfamiliar command — `unity command <name> --help`, never guess
- **`unity:*` skills** of the official plugin — knowledge and procedures, not actions

| Task | With |
|---|---|
| How to do it right (UI, URP, WebGL, audio, …) | `unity:*` skill — follow its procedure |
| Scene, GameObject, components, prefabs, materials, assets | **Coplay** |
| Serialized fields and object references | **Pipeline** `get_serialized_fields` / `set_serialized_field` |
| Project settings (Player, Quality, Physics, Time, Audio, Tags) | **Pipeline** `get_*_settings` / `set_*_settings` (`dry_run` first) |
| Build with BuildReport, `switch_build_target`, Project Auditor (`audit`), `get_performance_stats`, `capture_game_view` / `capture_scene_view`, Animator / Timeline, lighting / NavMesh / occlusion bake | **Pipeline** |
| Arbitrary C# | **Pipeline** `unity command eval '<code>'` |
| Tests and build with the Editor closed | `unity test` / `unity build` |

Do not connect the second Unity MCP server (`unity mcp`) — it duplicates Coplay.

## Only through MCP / Pipeline, never by hand

| Task | Tool (Coplay) |
|---|---|
| Scene load / save / hierarchy / selection | `manage_scene`, `find_gameobjects` |
| GameObject / components / properties | `manage_gameobject`, `manage_components` |
| Prefab create / edit / apply | `manage_prefabs` |
| Play mode / editor state | `manage_editor` |
| Console after scripts / domain reload | `read_console` |
| Asset search | `manage_asset` |
| Many similar mutations | `batch_execute` (one call instead of many) |
| Several Unity instances open | `set_active_instance` (exact `Name@hash`) before other calls |

Never rewrite `.unity`, `.prefab`, `.asset` with a manual diff, and never guess scene or hierarchy contents without querying the Editor. Neither Coplay nor Pipeline available → stop and tell the user.

## Sequences

- **Inspect a scene** — `manage_scene` (active / load) → hierarchy (`page_size` ~50, paging) → find target → `manage_components` with `include_properties=false` first
- **Change an object** — find target → `manage_gameobject` / `manage_components` → `batch_execute` for batches → save the scene if needed → `read_console`
- **Prefab** — `manage_prefabs` / `manage_asset` → edits through MCP (not YAML) → apply / save → `read_console`
- **Play mode** — `manage_editor` enter → `execute_code` `Application.runInBackground = true` (unfocused Editor does not tick) → check → exit before structural scene / asset edits (if the editor requires) → `read_console`. Never toggle `PlayerSettings.runInBackground` — writes `ProjectSettings.asset`, ships to WebGL

## Payload / tokens

- Hierarchy and components — paging, summary first
- `include_properties=true` — only when values are needed
- Asset search — small `page_size`; `generate_preview=false` by default
- Do not read `TextMeshProUGUI` / `TMP_Text` through Coplay component resources: the getter touches `fontMaterial` and creates material instances that get saved into the scene. Read TMP values with a read-only `unity command eval` (`fontSharedMaterial`, `font`, `text`, …)

## Pipeline and arbitrary C#

Before the first CLI use in a session — `unity status` (state `ready`, right project); `unity pipeline list` if the Editor does not answer (Safe Mode due to compile errors).

**Default — a typed command** (Coplay tool or named Pipeline command): fixed parameter schema, reviewable call, cannot do more than described.

**`unity command eval` — only when no typed command exists:**

1. Engine-specific API / internal state without a command (e.g. `SGG.PerfMeter.Editor.Mcp.PerfMeterMcpCommands.*` — skill `perfmeter`)
2. Read-only diagnostics with no command of its own
3. A one-off script for a narrow task clearly not covered by existing commands
4. Official `unity:*` skills hard-wired to `eval`

A mutating `eval` is arbitrary code with full `UnityEditor.*` / `UnityEngine.*` access: even if the permission mode lets it through, state it separately — the code does not show in advance what it will do.

**`unity test` / `unity build` — only with the Editor closed.** With the Editor open they fail here: `already open in a running Editor`, exit code 6. With the Editor open — `unity command run_tests` and `unity command build` (+ `build_status`). `unity run --command` reuses the open Editor and leaves it running.

**No need to announce (read-only):** Coplay — `read_console`, scene and hierarchy reads, `find_gameobjects`; Pipeline — `get_*`, `find_*`, `list_*`, `console`, `console_status`, `editor_status`; CLI — `unity status`, `unity logs`, `unity doctor`, `unity pipeline list`, `unity command` without a name (listing), `unity command <name> --help`.

If `unity status` does not see an open Editor — do not work around it (e.g. with a separate headless Editor): tell the user and clarify (possible causes: Safe Mode, agent sandbox).
