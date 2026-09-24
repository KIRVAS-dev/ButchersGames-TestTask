# ButchersGames-TestTask — Claude Code rules

`.cursor/` is a separate rule set for Cursor: do not read, sync or edit it.

**Reply to the user in Russian.** Rules, skills and reference docs are in English only to save tokens.

## Context

- **Unity 6.5**; WebGL demo on GitHub Pages, built with Android (Google Play) in mind — keep code valid for both; OOP, **not ECS**
- Scripts: `ButchersGames/Assets/_Project/Scripts/`; scenes **Bootstrap** (`ProjectScope`, loads Core) and **Core** (gameplay)
- Stack: VContainer (DI), UniTask (async), R3 (Model → Presenter → View), DOTween / FMOD (animation and sound in View). Check Unity API against Unity 6.5 docs
- `.claude/rules/` — auto-loaded when a `.cs` under `Scripts/` is read: architecture, design, codestyle, exceptions, rider-mcp. `unity-mcp.md` auto-loads only for `.unity` / `.prefab`; scene / prefab work through Coplay or Pipeline without reading those files → read `unity-mcp.md` manually first. `ui.md` auto-loads for `Scripts/UI`, `Prefabs/UI`, `Graphics/UI`, `Graphics/Fonts`; UI layout / sprites / fonts through Coplay or Pipeline → read `ui.md` manually first
- `.claude/reference/` — not auto-loaded; read when a rule points to it (new feature, Presenter, UI screen, …)
- Trigger-based procedures — skills in `.claude/skills/`: planning, review, refactor, perfmeter

## Approval

The session permission mode decides whether a tool may be called. The rules below decide what to do and how much.

### Report only

Message starts with `Review` / «ревью», `Refactor` / «рефактор(инг)», «анализ», «обоснуй», «сравни», `audit`, «проверь» (without «исправь»), or that word stands on its own line → report only, no mutating tools, even if the permission mode allows them. Edits come in a separate message («исправь», «примени»).

### Scope

- Do not extend edits beyond the named files / feature / layer without a new request
- A reworded task does not change an agreed plan by itself — ask when ambiguous
- `CLAUDE.md`, `.claude/` — touch only when explicitly part of the task
- git push, force operations, outward messages (issue, PR) — state them before calling, even if the permission mode would let them through
- Part of any `.cs` edit without asking: rename, format, lint, compile check and console (rider-mcp.md)

### Plan

Implement only after the plan is approved: `ExitPlanMode` or «го» / «ок» / «да» **with no new task** in the same message; new scope in the same message is approved separately.

### Proposal format

Default: **Суть** (1–2 sentences) → **План** (1–3 items) → **Затронутые файлы / системы**. If the action will hit a permission prompt anyway, do not duplicate it as a chat question; a large or ambiguous task — ask and stop until answered.

On «обоснуй» / «подробно» / «расскажи подробнее»: hypothesis and risks → options (pros / cons) → exact change list → rollback plan → approval request.

## Effort

A task touches many files (renaming serialized fields, changing event / interface signatures, code together with prefabs or scenes, rewriting several rule files) → before starting, warn that effort should be switched to high. Claude cannot change its own session effort, so after such a task explicitly remind the user to switch back to medium.
