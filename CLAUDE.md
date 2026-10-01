# ButchersGames-TestTask — Claude Code rules

`.cursor/` is a separate rule set for Cursor: do not read, sync or edit it.

**Reply to the user in Russian.** Rules, skills and reference docs are in English only to save tokens.

## Context

- **Unity 6.5**; WebGL demo on GitHub Pages (employer sees it first), Android (Google Play) later — keep code valid for both; not a playable ad; OOP, **not ECS**
- Base for a future game: the architecturally proper solution over test-task shortcuts; YAGNI still applies to speculative features
- Scripts: `ButchersGames/Assets/_Project/Scripts/`; scenes **Bootstrap** (`ProjectScope`, loads Core) and **Core** (gameplay + all visuals)
- Stack: VContainer (DI), UniTask (async), R3 (Model → Presenter → View), DOTween / FMOD (animation and sound in View). Check Unity API against Unity 6.5 docs
- `.claude/rules/` — path-scoped, load on Read: `.cs` under `Scripts/` → architecture, design, codestyle, exceptions, rider-mcp; `.unity` / `.prefab` → unity-mcp; `Prefabs`, `Graphics`, `Configs` → assets; UI paths → ui. Work through Coplay / Pipeline without reading such a file → read the matching rule first (hooks remind on create)
- `.claude/reference/` — read when a rule points to it; skills in `.claude/skills/`: planning, review, refactor, perfmeter, smoke, retro, commit

## Knowledge base

- Single source: `CLAUDE.md`, `.claude/rules/`, `.claude/reference/`, skills; rule candidates — `.claude/skills/retro/candidates.md`
- Auto-memory — inbox only: a fresh user correction goes there, retro moves it into the target file and deletes the entry. Never keep a rule both in memory and in the repo

## Task lifecycle

Every transition is offered via `AskUserQuestion`, never chained: do the approved step, report, stop.

| Stage | Action |
|---|---|
| New non-trivial task, mode not named | Dialog: Грилл (Рекомендуется) / Сразу план / Сразу реализовать |
| Trivial task | Proposal format; act on ok |
| Grill closed / plan exists | planning footer |
| Implementation approved | Task branch per commit skill «Branches»; never work in `main` |
| Plan done + checks passed (rider-mcp «After C# edits»; Play Mode after DI / scene / prefab changes) | Report → «Задача закрыта?»: Да / Продолжаем / Сначала проверю сам |
| «Да» | Skill `commit` |
| Committed | Dialog: Ревью (Рекомендуется) / Ретро / Стоп |
| Review has findings | Fixes on ok → skill `commit` → offer retro |
| Review clean / fixes committed | Dialog: Ретро (Рекомендуется) / Стоп |
| Retro done / «Стоп» on a task branch | Dialog «Слить в main?»: Да (Рекомендуется) / Позже; «Да» → «Кто сливает?»: Claude (Рекомендуется) / Я сам; Claude → merge per commit skill «Branches» |

- **Non-trivial** — any of: a decision the request does not settle (name, place, API, approach); unknown cause; > 1 file / object or code + prefab / scene; contract change (public API, signature, interface, serialized field, prefab structure). Unsure → non-trivial
- **Branching:** a request not needed for the current plan and touching another feature / layer / system → do not start it; dialog: Собрать контекст для нового чата / Делаем здесь / Отложить. Context — copyable block: goal, relevant decisions and constraints, `@` files, state (done / uncommitted), open questions; optionally `Prompts/handoff-<topic>.md`. «Отложить» → «Отложено» list in the final report

## Approval

The session permission mode decides whether a tool may be called. The rules below decide what to do and how much.

### Report only

Message starts with `Review` / «ревью», `Refactor` / «рефактор(инг)», `Retro` / «ретро» / «разбор сессии» / «итоги задачи», «анализ», «обоснуй», «сравни», `audit`, «проверь» (without «исправь»), or that word stands on its own line → report only, no mutating tools, even if the permission mode allows them. Edits come in a separate message («исправь», «примени»).

### Scope

- Do not extend edits beyond the named files / feature / layer without a new request
- A reworded task does not change an agreed plan by itself — ask when ambiguous
- `CLAUDE.md`, `.claude/` — touch only when explicitly part of the task; match the target file's style: terse English bullets, minimal diff, no explanatory prose
- Rule contradiction / stale fact that the current code settles → align the rule with the code; list the changes in one batch, ask only what the code does not settle
- git push, force operations, outward messages (issue, PR) — state them before calling, even if the permission mode would let them through
- Part of any `.cs` edit without asking: rename, format, lint, compile check and console (rider-mcp.md)
- Before stating how code, a prefab, a scene or a setting behaves — check it (code, git history, serialized fields via Pipeline, import `.meta`); «не проверено» only when checking is impossible (Unity closed, data outside the repo)
- A project rule blocks an option better by established practice (SOLID, DIP, encapsulation…) → say so: rule, principle, concrete risk here; propose the rule fix; follow the rule until approved. Options in architecture.md «Do not propose» and YAGNI trade-offs — not re-raised

### Plan

Implement only after approval: `ExitPlanMode`, «Реализовать» in the planning footer, or «го» / «ок» / «да» **with no new task** in the same message; new scope in the same message is approved separately. A question or suggestion («добавить X?», «а вариант с …?», picking an option number) is not a go and does not change the plan: answer, recommend, wait.

### Proposal format

Default: **Суть** (1–2 sentences) → **План** (1–3 items) → **Затронутые файлы / системы**. If the action will hit a permission prompt anyway, do not duplicate it as a chat question; a large or ambiguous task — ask and stop until answered.

Every question or offer to the user (incl. end-of-reply «сделать?», yes / no): first all questions numbered in the text (context, options, recommendation), then `AskUserQuestion` with the same numbers, short question and labels, no descriptions; recommended option first with «(Рекомендуется)»; > 4 options → split into another question of the same dialog; > 4 questions → dialogs in batches of 4. Dialog dismissed → wait for a text answer.

Task from a video / visual reference → before implementing: frame-based description (spawn, motion, lifetime, intensity) + one AskUserQuestion list of points not judgeable from frames; implement after the answers.

On «обоснуй» / «подробно» / «расскажи подробнее»: hypothesis and risks → options (pros / cons) → exact change list → rollback plan → approval request.

## Working style

- Rules first: the project rule answers → quote its line; general principles only where rules are silent. Check every new name, class and location against the rules and existing folders / asmdefs **before** writing
- Fix = root cause; a new responsibility without an owner → a new feature per feature-anatomy.md, not a chain of patches on existing classes
- Rank options by merit; mechanical follow-up (re-assigning serialized values, one more file) is a plan fact, not an argument for a worse option
- User-typed identifiers: fix typos, mention the corrected spelling once; unresolvable → ask
- Temporary test change on request: comment the original out, add the test code beside it, say how to revert
- File edits — Edit / Write, not scripts; never put a destructive command in the same call as a step that may fail
- Tool call failed / empty / needs retries → tell the user at once (what, likely cause, what is needed), no silent loops
- Web page unreadable via WebFetch → try Tavily (extract / search) before giving up
- URLs to open — markdown links, not backticks
- Platform render / quality settings: the Editor-tuned PC quality level is the reference; change only what the platform cannot support, list optional optimizations separately
- Before a long player build: list what goes in, ask about pending user edits (FMOD, scenes, settings)

## Effort

A task touches many files (renaming serialized fields, changing event / interface signatures, code together with prefabs or scenes, rewriting several rule files) → before starting, AskUserQuestion «⚠️ Переключите effort на high»: «Переключил» / «Продолжить с текущим»; end the turn, start the task on the answer. After such a task — AskUserQuestion «🔄 Верните effort на medium»: «Переключил» / «Оставить». Claude cannot change its own session effort.
