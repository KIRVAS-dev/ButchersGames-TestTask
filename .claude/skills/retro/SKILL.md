---
name: retro
description: Session retrospective — friction and its cause, task-statement quality, rule candidates via a journal. Use when the message starts with `Retro` / `retro` / «ретро» / «разбор сессии» / «итоги задачи» or that phrase stands on its own line. Report only, no edits.
---

# Retro

**Report only**; do not change code, rules, `CLAUDE.md` or the journal — writes need a separate ok. Not a review: review = code vs rules; retro = how the session went and which rule would have prevented the friction. Run before `/compact`. Report in Russian.

Basis: `CLAUDE.md`, `.claude/rules/`, `.claude/reference/`, skills; this session's facts.

## Evidence

- Only facts visible in this session: compile / test / console error, retry, rollback, file searched for, wrong tool, user clarification or correction. No fact — no item
- Context already compacted → say so; work only from the summary
- Forbidden: "train of thought", doubts, arguing with oneself without a visible fact; generic advice not tied to this project

## Friction causes

| Cause | Propose |
|---|---|
| No rule | Rule candidate |
| Rule exists but was not loaded (path-scoped rule not read, hook does not cover the case) | Fix loading: `paths`, hook, pointer in `CLAUDE.md` |
| Rule unclear / contradicts another | Rewording |
| Rule blocks a better option | Per `CLAUDE.md` Scope |
| An existing rule was broken | No new rule; check its loading and wording |
| Tool issue (Rider / Unity MCP) | `rider-mcp.md` / `unity-mcp.md` |

## Report

1. **Итог** — done / not done, 1–3 lines
2. **Трение** — episode → cause (table above); none → say so
3. **Постановка задачи** — what was missing or ambiguous in the request; «замечаний нет» is allowed, do not invent
4. **Как поставить лучше** — short rewrite of the request; only if section 3 has remarks
5. **Кандидаты в правила** — 0–3: rule, why (episode), target file
6. **Из памяти в правила** — every auto-memory entry (inbox, CLAUDE.md Knowledge base) and every explicit user correction of this session: rule text and target file; after the move the memory entry is deleted
7. **Коммиты** — uncommitted work of the session → proposed commit(s) per `.claude/skills/commit/SKILL.md` Convention; nothing uncommitted → omit

Target file: code — `architecture` / `design` / `codestyle` / `exceptions` (+ `reference/feature-anatomy.md`); MCP — `rider-mcp` / `unity-mcp`; UI — `ui`; workflow / approval — `CLAUDE.md`; procedure — the matching skill.

## Journal `candidates.md`

- After the report — offer via `AskUserQuestion` (`CLAUDE.md` Proposal format): section 6 items straight into their target files, section 5 candidates into the journal (by number, multiSelect), commits if section 7 exists; write only after ok
- Explicit user corrections skip the journal and its counter; the journal holds only Claude's own friction hypotheses
- Same target file and meaning already there → no duplicate: counter +1, add date and episode
- Counter reaches 3 → offer to move into the target file and remove from the journal; only after ok. The user may promote earlier
- Moving into a rule: terse English bullet in the target file's style, minimal diff; why and example stay in the journal

**Report only**; journal and rule writes need a separate ok.
