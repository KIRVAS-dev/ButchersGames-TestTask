---
paths:
  - "ButchersGames/Assets/_Project/Scripts/**/*.cs"
---

# Rider MCP and post-edit checks

One MCP server `rider` (official JetBrains Rider MCP). This rule governs rename, refactorings, format, lint and the post-edit sequence. Other server tools (search, navigation, build, profiler, debugger) are allowed and unregulated.

Scene / prefab / components — [unity-mcp.md](unity-mcp.md) (read it before Editor work). Style — [codestyle.md](codestyle.md).

`rootFolder` — the Unity project root holding the `.sln` (`ButchersGames/`) — pass it in **every** call: the Rider server is one per IDE, and without `rootFolder` a call may hit another open solution.

## Connection

Before the first `.cs` edit in a task, make sure `rider` is connected (`mcp__rider__*` tools exist and a call answers). Not connected / not answering → **stop, tell the user and wait**:

- the user fixed the cause (Rider started, MCP server enabled, reconnect via `/mcp`) → continue
- the user explicitly allowed working without Rider → do not run rename or refactorings from this rule (say at once which task steps this blocks); skip format and lint; add a separate report line "Rider unavailable: format / lint not run"

The permission lasts until the end of the task; check again in the next task.

## Rename

Renaming C# symbols (type, method, property, field, parameter, …) — **only** `mcp__rider__rename_refactoring`. **Forbidden** for rename: Edit / project-wide find-replace; rewriting a file with a new type name; `git mv` / manual `.cs` rename instead of Rider when the file is tied to a type.

| Argument | Value |
|---|---|
| `filePath` | `.cs` declaring the symbol (absolute or solution-relative) |
| `symbolName` | `TypeName`, `MemberName` or `TypeName.MemberName` |
| `newName` | new name |
| `rootFolder` | see above |

- Unsure of the target symbol / public Api → first `preview: true`, check `affects`, then apply. `preview` does **not** run conflict analysis — a clean preview can still give `ok=false` on apply
- `ok=false` with `conflicts` — rejected, files untouched; adjust `newName`, never work around with a diff. `error.kind` (`new_name_invalid`, `no_renamable_symbol`, `new_name_matches_current`, `symbol_invalidated`, …) names the cause — do not guess
- On success Rider updates references itself (`touched`). Manual pass over references — only on error / partial result

**Renamed `[SerializeField]` field:** never add `[FormerlySerializedAs]`. Find the scene and prefab objects that lost values or references, read them with Pipeline `get_serialized_fields`, re-assign with `set_serialized_field`, save the scene / prefab ([unity-mcp.md](unity-mcp.md)). This is a multi-file task — see CLAUDE.md, "Effort".

## Refactorings

Structural refactorings — **only** through Rider; a manual diff is forbidden for the same reasons as for rename (missed call sites, overrides, references).

| Task | Tool |
|---|---|
| Add / remove / reorder method parameters | `change_api_signature` |
| Delete a type or member | `safe_delete` |
| Change a type's namespace | `move_type_to_namespace` |
| Extract statements into a method | `extract_method` |
| Extract an interface | `extract_interface` |
| Extract a base class | `extract_base_class` |

- `preview: true` first, check the result (`affects`, `newSignature`, `conflicts`, `warnings`), then apply
- `ok=false` / `conflicts` — nothing changed; adjust arguments or tell the user, never work around with a diff
- `safe_delete` with `conflicts` — the symbol has usages; do not delete by hand, decide about the usages first
- `change_api_signature`: a removed parameter is **not** cleaned from the method body — go through `warnings`; several overloads → `currentSignature` / `declaringType`
- `extract_interface` / `extract_base_class` create the type **in the same file** → move it to its own file per [architecture.md](architecture.md) (interface into `Api/`), then format

## After C# edits

After the last `.cs` edit of the task (create or edit), before the report:

1. `mcp__rider__reformat_file` — **one** call with `files` = all touched files
2. `mcp__rider__lint_files` — same `files`, `min_severity: warning`
3. Coplay `refresh_unity` (`compile`, `wait_for_ready`) — wait for compilation to finish (`isCompiling` in editor state). Format changes files and may retrigger compilation, so it goes **before** refresh
4. Coplay `read_console` — Errors; fix before any step that depends on the new types
5. If needed — EditMode tests: `unity command run_tests` (Editor open) or `unity test` (Editor closed only)
6. New components / play mode — only after a clean compile

Paths in `files` — **relative to the solution root**, not absolute.

`lint_files` results:

- error — fix, then format + lint the fixed files again
- warning on lines changed in the task — fix
- warning in untouched code — do not edit, list it in the report
- `timedOut` / `more: true` / `notAnalyzedReason` — rerun for the remaining files; do not treat the file as clean

**Forbidden:** rewriting a whole file for whitespace, wrapping or member order.

`reformat_file` / `lint_files` unavailable mid-task → as in "Connection": stop, tell the user; never substitute format with a manual diff or mark the step done.

## Exceptions

| Area | Tool |
|---|---|
| `.unity`, `.prefab`, `.asset`, `.asmdef` | Coplay / Pipeline ([unity-mcp.md](unity-mcp.md)) or a targeted diff per task |
| New file with a new type (not rename) | create the file; format + lint at the end of the task |
| Configs outside `Assets/` | Rider not required |
