---
name: planning
description: Grill interview and plan before implementation. Use on triggers `grill me` / `grill` / «погриль» / «составь план» / «план» (starts the interview) and in Plan mode (`EnterPlanMode`) until approval. Outside planning do not flood the user with questions.
---

# Planning (grill-me)

Platform, layers, style — `.claude/rules/` (architecture, codestyle, design, exceptions) and `.claude/reference/feature-anatomy.md` for features; do not duplicate here. Questions, options and the footer are shown to the user — write them in Russian.

## Pre-flight (before the first decision question)

Pin down or clarify 2–4 inputs in **one** batch (`AskUserQuestion`):

- scale / numbers, if they matter
- previous attempts — what was tried and why it was rejected
- constraints — what must not change (API, prefab, scene)
- everything else from context and architecture.md — state it explicitly, do not re-ask

A new input **mid-grill** → stop, re-plan the question tree.

## Grill rules

1. One decision question at a time, via `AskUserQuestion`. Tool unavailable → say so and offer a text answer (number / option label)
2. 2–4 options; recommended one first, «(Рекомендуется)» in its label
3. Adapt the tree — drop questions that lost their point
4. Answer is in the code — read the repo, do not ask; in doubt — codebase / Context7 / docs / search
5. Short context (1–2 lines); no process chatter ("now Q3")
6. **Do not just agree** — contradictions with code, rules or the plan — with a quote; do not ignore the user's counter-arguments
7. Cross-check: a new answer must not break what is already decided

## Close before the plan

- Scope (in / out)
- API shape (names, types, configuration)
- Lifecycle (creation, destruction, phases)
- Edge cases (interruptions, repeated input, busy)
- Adjacent mechanics (architecture / neighbouring features)
- Files and layers, order of changes
- Risks and verification (gameplay scene / TEST — per project)

## Pipeline: questions → readiness → footer

```
questions (one at a time)
        │
        ▼
agent is confident: no open branches
   (or user: «всё» / «хватит» / «составляй план»)
        │
        ▼
show footer (AskUserQuestion / fallback)
        │
        ▼
act only on the chosen option / code phrase
```

- While in doubt — **only questions**, no footer
- **Never** auto-run plan / implement / copy / save after readiness

## Footer after readiness

One footer for both stages (grill closed and plan exists). Footer = **`AskUserQuestion`** with options in fixed order:

1. «го» / `implement` — execution (code) — **only if** the plan is already fixed (`ExitPlanMode`); otherwise create the plan first
2. `create plan` / «покажи план» / «составляй план» — fix or re-show the plan (`EnterPlanMode` → `ExitPlanMode`)
3. `grill me` — more questions
4. `copy prompt` — copyable `[TODO]` block
5. `save prompt` — `Prompts/<feature>.md` outside `Assets/`

Tool unavailable → say so; same list as text; offer an answer by number / phrase.

Before code — **wait** for option 1 / «го» / implement / explicit ok **after** plan approval.

## Code phrases: copy / save prompt

| Action | Phrases (equivalent) |
|---|---|
| Copyable block | `copy prompt`, «скопируй промпт», «копируемый промпт» |
| `Prompts/` file | `save prompt`, `save prompts`, «сохрани промпт», «сохрани в prompts» |

These do **not** trigger a prompt: «сохрани план», `create plan`, «составляй план».

### Prompt format (`copy prompt` / `save prompt`)

- Context block: `@` only the needed files
- Every change step — `[TODO]`
- Exact signatures (name, parameters, return type), class and file names
- Style: dry, technical; no motivation, preambles or alternatives; text in Russian
- At the end: `Проверка: …`
- `copy prompt` — a separate copyable code block in chat
- `save prompt` — same content in `Prompts/<feature-kebab-case>.md` (**outside** `Assets/` — Unity does not import it); update the file when the plan changes

Template:

```
Контекст: @Path/To/FileA.cs @Path/To/FileB.cs

[TODO] 1. FileA.cs: добавить метод `ReturnType MethodName(ArgType arg)` — <что делает>
[TODO] 2. FileB.cs: изменить `ExistingMethod` — <точное изменение>
...
Проверка: <как убедиться, что работает>
```
