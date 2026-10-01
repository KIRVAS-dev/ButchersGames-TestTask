---
name: commit
description: Commit convention and commit procedure — message format, types, task branches and merge into main, one commit per task, WIP commits, fixup / squash / reword / reorder of unpushed commits. Use when the message is «сделай коммит» / «закоммить» / `commit`, «WIP-коммит» / «закоммить WIP», «причеши историю» / «оптимизируй историю», or «слей в main» / merge of a task branch. Planning, review and retro read the Convention section to suggest commit messages.
---

# Commit

Proposals and approval are shown to the user — write them in Russian; commit messages are in English.

## Convention

### Header

`type(scope): subject`

- English; imperative (`add`, not `added` / `adds`); lowercase after the colon; no trailing period; ≤ 72 chars
- Breaking change (save format, public API between assemblies) → `type!:` + `BREAKING CHANGE:` footer
- No `Co-Authored-By` trailer

### Types

Pick the first that fits, top to bottom:

| Type | When |
|---|---|
| `revert` | Reverts an earlier commit; body `This reverts commit <sha>.` |
| `feat` | New behaviour: mechanic, system, screen, editor tool — code together with the prefabs / scenes / configs it needs |
| `fix` | Wrong behaviour made right |
| `perf` | Faster / less memory / fewer draw calls, same behaviour |
| `balance` | Values only: configs, ScriptableObjects, numbers in prefabs / scenes; no logic or structure change |
| `content` | New or changed assets on existing systems: levels, props, models, materials, VFX, sounds, lighting, UI graphics; no new code |
| `refactor` | Structure changes without behaviour change, incl. formatting |
| `test` | Tests only |
| `docs` | Documentation only; `CLAUDE.md` and `.claude/` → `docs(rules)` |
| `build` | Project / player / build settings, packages, asmdef references, Unity version; third-party plugin / Asset Store / UPM import → `build(deps)` |
| `ci` | CI / CD, deploy to GitHub Pages |
| `chore` | Nothing above fits: `.gitignore`, `.editorconfig`, repo housekeeping |

### Scope

- No fixed list — the narrowest name that covers every touched place (feature, system: `gates`, `runner`, `ui`, `audio`)
- No sensible generalisation → omit the scope; list the systems in the body

### Body

- Only when there is a «why»: required for `fix` (cause), `balance` (old → new value and reason), non-obvious decisions, commits across several systems
- Why, not how — the diff shows how; wrap at 72 chars

### WIP

- Temporary commit to save or pin an intermediate state of a task: `WIP: <free text>`
- Convention, split and «Commit content» do not apply — do not check or criticise the content (also in review / retro)
- **Never pushed**: before a push, the unpushed commits have a WIP → stop, propose squash / fixup into the final commit of the task. Local `.git/hooks/pre-push` rejects subjects starting with `wip` (any case); not versioned — do not bypass with `--no-verify`
- «WIP-коммит» / «закоммить WIP» → commit the task's changes as `WIP: …` without the approval list

## Branches

- No work in `main`: every task, trivial too — own branch `<type>/<task>`; type from «Types» — the task's expected commit type; task — English kebab-case, 2–4 words (`feat/door-animation`, `docs/task-branches`)
- Claude creates it when implementation starts (`CLAUDE.md` Plan approval), from local `main`: clean tree, scenes / prefabs saved in the Editor; otherwise stop and ask
- Already on the task's branch → continue; on another task's branch → stop and ask
- Task commits = `main..HEAD`
- Merge — only after the task closes (`CLAUDE.md` Task lifecycle): commits per «Commit content» (squash / fixup first) → `git switch main` → `git merge --ff-only <branch>` → `git branch -d <branch>`; not fast-forward → `git rebase main` on the branch first, conflict → `git rebase --abort`, report
- Push of a branch or `main` — only on a separate command

## Commit content

- One task — one commit by default; split only for an unrelated responsibility (another feature, unrelated housekeeping, third-party import, large mechanical refactoring such as a project-wide rename)
- Refactoring inside the feature's code or needed by it → in the feature commit
- Asset and its `.meta` in the same commit; move / rename — both paths together
- Code and the prefabs / scenes / configs depending on its serialized fields or components → same commit; no intermediate commit with Missing Script / empty references
- Third-party import → its own `build(deps)` commit, no own changes
- Regenerated files (FMOD banks, lightmaps, atlases) → with the change that regenerated them
- One file with changes of unrelated responsibilities (e.g. `Core.unity`: feature + an accidentally moved object) → stop, show the unrelated hunks, ask: revert / separate commit / keep. Do not split Unity YAML by hunks

## Procedure: «сделай коммит» / «закоммить»

1. `git status`, `git diff`, `git diff --cached`, `git log main..HEAD`; on `main` → stop, create the task branch per «Branches» first
2. Group changes by responsibility; check every file against the task and «Commit content»
3. Unpushed commits — always check and propose in the same list: commits of the same task (incl. WIP) → squash into one; new changes finish an existing commit → fixup; message breaks the Convention or no longer matches the content → reword. Rewriting runs per «причеши историю»
4. Show **one** list:
   - per commit — header, body (if any), files
   - «Не войдут» — unrelated files with the reason and a proposed commit / `.gitignore` / revert for each
5. One approval (`AskUserQuestion`: commit all / merge into one — when > 1 commit / edit / split differently) → commit in order; edits to the list come as text
6. `git push` — never as part of this skill; only on a separate command, stated before the call (`CLAUDE.md` Scope)

## Procedure: «причеши историю» / «оптимизируй историю»

- Unpushed task commits (`main..HEAD`, not on the remote): propose reorder / squash / fixup / reword where the result reads as one meaningful step or removes noise (commits of one task, `fix` of an unpushed `feat`, a message that does not match the content)
- Pushed: advice only, marked «нужен force push — делаете вы». Force push is **never** run by Claude
- Approval → backup branch `backup/<yyyy-MM-dd-HHmm>` → non-interactive rebase: `git commit --fixup=<sha>` + `git rebase --autosquash`, or a todo file in the scratchpad via `GIT_SEQUENCE_EDITOR` (`-i` is unavailable)
- Conflict → `git rebase --abort`, report; do not resolve on your own
- After success: show the new `git log main..HEAD`; offer to delete the backup branch

## Suggesting commits from other skills

- planning — section «Коммиты» in the plan: by default one header for the task
- review — only when uncommitted changes were reviewed: proposed commit(s) at the end of the report
- retro — proposed commit(s) for uncommitted work of the session
