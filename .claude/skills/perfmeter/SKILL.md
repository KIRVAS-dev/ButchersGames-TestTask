---
name: perfmeter
description: Runtime performance measurement via SGG PerfMeter and Unity Pipeline (`unity command eval`) with a frame-budget report. Use when the message starts with `Perf` / `PerfMeter` / «перф» or that word stands on its own line.
---

# PerfMeter

Kit for **SGG PerfMeter** (`com.sungeargames.perfmeter`). Platform, scenes, stack — CLAUDE.md and `.claude/rules/architecture.md`. Unity Editor bridge — `.claude/rules/unity-mcp.md`. Need a measurement plan — skill `planning`. Report in Russian.

## Access

- PerfMeter has **no** MCP server of its own
- Main data path: **Unity Pipeline** → `unity command eval '<code>'` → `SGG.PerfMeter.Editor.Mcp.PerfMeterMcpCommands.*` (Coplay `execute_code` is in the disabled `scripting_ext` group — do not use)
- Zero-code settings: `Assets/Resources/SGG.PerfMeter/perfmeter-settings.json`
- Setup UI: **SGG → Perfmeter → Setup** (toggles **Setup → Work mode**: Enabled / Auto Start / Collect Metrics / Show Overlay)

Call (bash, code in single quotes; result in `data.result.result` of the JSON):

```bash
unity command eval --format json --no-banner 'return SGG.PerfMeter.Editor.Mcp.PerfMeterMcpCommands.MetricsLatest();'
```

PerfMeter commands for `eval`:

```csharp
return SGG.PerfMeter.Editor.Mcp.PerfMeterMcpCommands.SetupStatus();
return SGG.PerfMeter.Editor.Mcp.PerfMeterMcpCommands.RuntimeStatus();
return SGG.PerfMeter.Editor.Mcp.PerfMeterMcpCommands.MetricsLatest();
return SGG.PerfMeter.Editor.Mcp.PerfMeterMcpCommands.DeviceInfo();
return SGG.PerfMeter.Editor.Mcp.PerfMeterMcpCommands.AlertsLatest();
return SGG.PerfMeter.Editor.Mcp.PerfMeterMcpCommands.RuntimeModeSet("{\"mode\":\"Background\"}");
return SGG.PerfMeter.Editor.Mcp.PerfMeterMcpCommands.SessionStart("{\"warmup_seconds\":2,\"sample_interval_seconds\":0.25,\"max_samples\":240}");
return SGG.PerfMeter.Editor.Mcp.PerfMeterMcpCommands.SessionExport("{\"format\":\"json\",\"path\":\"Temp/PerfMeter/session.json\"}");
```

Also without `eval`: `unity command get_performance_stats` (render / memory / frame timing), `capture_game_view`.

### Config and Setup

- `perfmeter-settings.json` — live source of defaults; the package rereads it during session / alerts / overdraw
- **`setup.run` / the Setup wizard overwrites the JSON** with its defaults (often `autoStart=true`, overlay on). After any Setup, re-apply the measurement baseline (see Pre-flight); do not rely on the file "as it was"
- Non-empty `overlayPresets` + empty `activeOverlayPresetId` → falls back to the first overlay preset

## Metric layers and environments

| Layer | Examples | Reliable in |
|---|---|---|
| Frame Timing | FPS, CPU / GPU frame time | Editor, Dev Build, player (if Frame Timing Stats on) |
| ProfilerRecorder | draw calls, SRP, memory, GC, overdraw counters | **Editor / Development Build**; in a plain release often `unavailable` |

For this project:

- **Default measurement:** Editor Play Mode on the target gameplay scene (Core) + Pipeline `eval`
- Editor / Dev Build → counts and **relative** comparisons (A/B); absolute ms for the target platform (WebGL) — **with a caveat**
- Confirm a ship / fix verdict on the **target player** (WebGL build) when the numbers decide it

## Hypothesis → preset (no full grill)

If the user named a bottleneck — pick a narrow preset / modules; otherwise baseline + a broad glance:

| Hypothesis | Preset / focus |
|---|---|
| General baseline / «просто сними» | Background, overlay off; then a Timing glance |
| CPU-bound | Timing |
| Draw calls / batching | Rendering (+ SrpBatcher) |
| Memory / GC | Memory |
| Overdraw | a short Overdraw / heatmap diagnostic window **only**, not baseline |

Overlay choice affects UI / DC noise; session sampling — barely. For a clean baseline the overlay is **off**.

## Pre-flight

1. `unity status` — Editor `ready`; Play Mode on the target gameplay scene (Core)
2. `SetupStatus` — Frame Timing Stats on; JSON loaded if zero-code; URP Render Graph feature on the **active** renderer (recheck after URP edits)
3. **Baseline normalization:** collection **Background**; overlay **hidden** (or Minimal only if a human glance is needed); FMOD Debug / other debug UI off — they inflate Draw Calls
4. Editor **focused**, not paused; otherwise Frame Timing may be `NotCollected`
5. `OverdrawDiagnostic` / heatmap — only short diagnostic windows, not baseline
6. State the environment explicitly in the report: Editor ≠ target player (WebGL) ≠ device
7. Package missing / handlers do not compile → stop and say so; never parse overlay or Console screenshots instead of JSON

## Procedure (Editor + Pipeline — main path)

1. `RuntimeEnsure` / `RuntimeStatus`
2. If needed `RuntimeResetStats`; warmup **≥2 s** (or `warmup_seconds` in `SessionStart`)
3. Take `MetricsLatest`, `DeviceInfo`, `AlertsLatest`; optionally camera / rendergraph snapshot
4. A/B or artefact: `SessionStart` → scenario → `SessionStop` / `SessionExport`
   - Fit `sample_interval_seconds` and `max_samples` to the duration (e.g. 240 × 0.25 s ≈ 1 min; longer — raise interval or maxSamples)
5. Match metrics to the scene through Coplay / Pipeline (renderer / UI counts etc.), do not guess
6. Do not mix lifecycle alerts at start (`fps.below_target` etc.) with steady state without warmup

In the Editor the agent starts / stops the session **itself** via `eval`. No on-screen button is needed for this path.

## Manual bracket in a player (optional)

Only on an explicit phrase: «ручной замер на устройстве» / «кнопка» / «брекет в билде» (and similar). A plain `Perf` does **not** start this path; without such a request do not put a TEMP button into `Assets/`.

Needed when there is no Editor bridge at runtime (Development / target WebGL player) and the user sets the window with START / STOP taps.

**Code source (do not invent or generate from scratch):** `.cursor/prompts/perfmeter/PerfMarkDebugButton.cs.txt` — the one allowed read from `.cursor/`, only for this path.
The template lives outside `Assets/` — Unity does not import it. The build has no button until a copy is placed in the project and the player is built.

### Cycle

1. Copy the template → `ButchersGames/Assets/Debug/PerfMark/PerfMarkDebugButton.cs` (create the folder if needed)
2. Wait for compilation; `read_console` — no Errors
3. Build a **Development Build** WebGL. Plain release: button and glue compile out under `#if DEVELOPMENT_BUILD || UNITY_EDITOR || PERFMETER_RELEASE`; for a deliberate release profile — define `PERFMETER_RELEASE` before the build
4. Run the player. The script creates the overlay button itself (`RuntimeInitializeOnLoadMethod`) — do not touch the scene / prefab
5. User: START → plays a segment → STOP. Log: `[PERFMARK]` (+ `[TEMP:perfmark]`). JSON: `Application.persistentDataPath/PerfMarks/session_n.json` (exact `path` in the STOP marker)
6. Fetch the export: on WebGL the path is platform-specific (browser storage); ask the user for the file / export method, do not assume adb
7. Parse JSON + markers; report as below. The button's Canvas inflates DC — do not mix with a "clean" baseline without a caveat
8. **Cleanup (mandatory):** delete `Assets/Debug/PerfMark/PerfMarkDebugButton.cs` (+ `.meta` and the empty folder); grep the repo — no `[TEMP:perfmark]` or `PerfMarkDebugButton` left. Do **not** delete the template in `.cursor/prompts/`

Do not keep the button in `Assets/` between measurements. Editor + Pipeline baseline — without the button.

## Reading metrics

| Signal | Fields |
|---|---|
| Budget | `target_fps` / `frame_budget_ms` (60 → 16.67 ms) |
| FPS | `average_fps`, `one_percent_low_fps`, `point_one_percent_low_fps`, spikes |
| Bottleneck | `bottleneck`: CPU main / render / GPU / present/VSync / Balanced / unknown |
| Timings | `cpu_*_frame_time_ms`, `gpu_frame_time_ms` (if available) |
| Rendering | `draw_calls`, `set_pass_calls`, `vertices`, `srp_batcher_instances` |
| Memory | `system_used_memory_bytes`, `gc_reserved_memory_bytes`, `gpu_memory_bytes` |

### Pitfalls

- **`batches = 0`** — classic Dynamic / Static / Instanced; normal with Dynamic Batching off. SRP Batcher → `srp_batcher_instances`
- High Draw Calls at ~1:1 Sprite / Mesh / UI — weak batching / materials / many Canvases
- Unfocused / paused Editor → Frame Timing may be `NotCollected`
- After Setup the JSON may have been reset — check overlay / mode before trusting numbers

## Report format

- Verdict **first**: within budget / risks / over budget (target FPS / WebGL — from the request or project context)
- Environment: Editor / target player / device, scene, collection mode, overlay on / off, focused. Never present an Editor measurement as target player / device
- Table: FPS, 1% / 0.1% low, bottleneck, CPU / GPU ms, DC, SetPass, SrpInst, memory
- Findings: `Severity (Blocker / Major / Minor / Nit)` — area / metric — issue — reasoning — recommendation
- Only justified remarks; **no code edits** until the user asks
- Optimization ideas — a separate list of proposals

## Prompt for a new chat (copy)

```
Perf

Scene: Core (gameplay). Play Mode. Target: target FPS / WebGL.

Take SGG PerfMeter metrics via Unity Pipeline (unity command eval) (Background, overlay off),
warm up ≥2 s, read metrics/status/device/alerts.
Report: budget verdict, metrics table, findings with severity,
caveat Editor ≠ target player. No code edits until I ask.
```
