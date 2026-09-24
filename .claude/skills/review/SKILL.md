---
name: review
description: Review project code for rule compliance and bugs. Use when the message starts with `Review` / «ревью» / `review` or that word stands on its own line. Report only, no edits.
---

# Review

Rules to check against: `.claude/rules/` — architecture, design, codestyle, exceptions, rider-mcp (+ unity-mcp for scenes / prefabs) and `.claude/reference/feature-anatomy.md` for feature structure, roles, Presenter / UI screens. Read whatever is not in context before starting. Report in Russian.

## Scope

- Default — the whole project (`ButchersGames/Assets/_Project/Scripts/` by layer)
- User set a scope — review **only** it, do not extend
- Commit / range — `git log` / `git show`; read files in scope **in full**, not just the diff

## Categories

- **Architecture** — layers, dependency rule, DI, roles — architecture.md, feature-anatomy.md
- **Design** — classes, methods, variables, assembly visibility — design.md
- **Code style** — codestyle.md
- **Exceptions / Guard / Validate / soft-checks** — exceptions.md, architecture.md
- **Bugs and logic:** null, async / `CancellationToken`, leaked subscriptions and tweens, edge cases
- **Performance:** allocations on hot paths, `GetComponent` / `Find` in frequent code, redundant updates; doubts about API / perf → docs / Context7. Full runtime measurement — skill `perfmeter`
- **C# / engine API:** current for Unity 6.5, nothing Deprecated

## What counts as a finding

Only a real violation in scope: a rule violation, a bug or a measurable degradation. Taste and "for SOLID / cleanliness" advice are not findings. Typical design findings:

- **YAGNI:** a type, layer, parameter or flexibility "for the future" without a second consumer
- **Hot path:** scene search, repeated `GetComponent`, extra allocations, LINQ — only if the code really is on a hot path
- **Inheritance** to reuse code instead of an "is-a" relation
- Public interface member on the concrete type although all consumers use the port and explicit would be painless; explicit event with a backing field for formality
- Method length alone is not a finding — a blurred purpose or deep nesting is

## Findings format

- Verdict **first**: compliant / has remarks / blockers
- `Severity (Blocker / Major / Minor / Nit)` — `File:line` — issue — reasoning — recommendation
- No findings → say «без замечаний» explicitly

**Report only**; edits need a separate ok.
