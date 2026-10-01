---
name: smoke
description: Smoke test of the WebGL player in the user's Chrome — optional build → local server → Play → in-game start → 30 s run → console check, pass / fail report. Use when the message starts with `Smoke` / «смоук» or that word stands on its own line, and right after any WebGL player build made by Claude.
---

# Smoke (WebGL)

Unity Editor bridge — `.claude/rules/unity-mcp.md`. Report in Russian.

## Mode

| Trigger | Tested build |
|---|---|
| `Smoke` / «смоук» | `docs/` (current Pages build), no build |
| `Smoke` + «сборка» / `build` | build into `Builds/Smoke/` first, then test it |
| Claude just built a WebGL player | that build's output folder |

`<repo>` = `E:/Unity/Projects/ButchersGames-TestTask`. `Builds/` is gitignored; never build the smoke player into `docs/`.

## 1. Build (build mode only)

1. Ask about pending user edits (FMOD, scenes, settings) before the build; wait for the answer
2. `unity status` — Editor `ready`, right project
3. Debug symbols on (mutating `eval` — state it before the call); remember the returned value:

```bash
unity command eval --format json --no-banner 'var old = UnityEditor.PlayerSettings.WebGL.debugSymbolMode; UnityEditor.PlayerSettings.WebGL.debugSymbolMode = UnityEditor.WebGLDebugSymbolMode.Embedded; return old.ToString();'
```

4. `unity command build --target WebGL --outputPath <repo>/Builds/Smoke --confirm` → poll `unity command build_status` until `completed` (poll interval ≥ 30 s)
5. Restore the remembered mode via the same `eval` (`= UnityEditor.WebGLDebugSymbolMode.<old>`) — also when the build failed
6. BuildReport has errors → **fail**, skip the browser; report the errors

Other Player / linker settings stay as they are: the smoke player must match the release build except for symbols.

## 2. Server

- `python -m http.server 8765 --directory <tested folder>` — Bash, `run_in_background`
- Port busy → stop and tell the user; do not pick another port

## 3. Chrome

**Claude in Chrome** only (`mcp__claude-in-chrome__*`, user's real Chrome with GPU). Not the built-in browser pane — it renders the build wrong. Extension not connected → stop and say so.

1. One ToolSearch: `tabs_context_mcp`, `tabs_create_mcp`, `navigate`, `javascript_tool`, `computer`, `read_console_messages`, `tabs_close_mcp`
2. New tab → `http://localhost:8765`
3. Before any click, install the hook (`javascript_tool`). The template calls `alert(message)` on load failure — a modal dialog blocks Chrome control:

```js
window.__smoke = [];
const push = (level, args) => window.__smoke.push({ level, text: args.map(String).join(' '), t: performance.now() });
for (const level of ['error', 'warn']) {
    const original = console[level].bind(console);
    console[level] = (...args) => { push(level, args); original(...args); };
}
window.alert = (message) => push('error', ['[alert]', message]);
window.addEventListener('error', (e) => push('error', ['[onerror]', e.message, e.filename + ':' + e.lineno]));
window.addEventListener('unhandledrejection', (e) => push('error', ['[unhandledrejection]', e.reason]));
```

4. Click `#play-button` («ИГРАТЬ»)

## 4. Scenario

1. Load: poll `window.isStarted === true` (set by the template in `createUnityInstance().then`), limit **60 s** → otherwise **fail** «загрузка не завершилась»; note load time
2. In-game start: screenshot → find the start button of the start screen on the canvas → click it → second screenshot confirms the run began (start screen gone). Button not found / run not started → **fail** «старт не нажат»
3. Run **30 s** without input
4. Final screenshot

## 5. Check

Sources: `window.__smoke` + `read_console_messages` (dedupe).

| Fail | Report only |
|---|---|
| `RuntimeError`, `Exception`, `null function`, `Aborted`, `[alert]`, `[onerror]`, `[unhandledrejection]`, any `console.error` | warnings |

## 6. Cleanup (always, pass or fail)

- Close the tab (`tabs_close_mcp`), stop the background server
- Build mode: `git status --short -- ButchersGames/ProjectSettings` — `ProjectSettings.asset` changed → restore symbols mode, tell the user

## Report

- Verdict **first**: ✅ прошёл / ❌ упал (stage: сборка / загрузка / старт / забег)
- Tested build: `docs/` or `Builds/Smoke/` (+ build time, size from BuildReport)
- Load time, run duration
- Errors — full text with stack (symbolized in build mode); warnings — list, grouped
- Final screenshot
- No code edits; fixes — a separate proposal
