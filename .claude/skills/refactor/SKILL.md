---
name: refactor
description: Code-structure analysis and a catalogue of refactoring recommendations (Code Complete 24.3 adapted to the project). Use when the message starts with `Refactor` / `refactor` / `refactoring` / «рефактор» / «рефакторинг» or that word stands on its own line. Report only, no edits.
---

# Refactor

**Report with recommendations only**; do not change code, git or scenes — edits need a separate ok. Not a review: review = rule compliance and bugs; refactor = catalogue of structural improvements. Report in Russian.

Basis: `.claude/rules/` — architecture, design, codestyle, exceptions; `.claude/reference/feature-anatomy.md` for roles. At implement time, renames and structural refactorings go only through Rider (rider-mcp.md).

## Scope (must be resolved)

Priority top to bottom; **do not extend** beyond it:

1. **Explicit paths / attachments** in the message
2. **File / feature / folder names** given by the user
3. **Commit / diff files** — if the user said «в коммите», «в diff», «staged», «последний коммит»; otherwise ask
4. Scope unclear → **one** clarifying question; never refactor "the whole project" or polish a module "to perfection" without bounds

Read files in scope **in full**, not just the hunk.

## Procedure

1. Fix the scope (list of paths)
2. Go through the catalogue; a finding only for a **real** smell in scope
3. Respect the project: Model / Service / View layers, InputHandler, UniTask, typed exceptions, `_camelCase`, Rider format
4. **Do not** propose: micro-optimizations without evidence; taste edits; public Api changes without need; reordering `[SerializeField]` without reason; a full compliance audit instead of refactoring (that is review)
5. Deliver the report; stop until ok to implement

## Catalogue

### Data / variables

| Smell | Recommendation |
|---|---|
| Magic number / literal | Named `const` / `static readonly` / config SO |
| Bad / short name (`temp`, `x`, `data`) | Rename (Rider); one purpose per variable |
| Complex unnamed expression | Explaining local variable |
| One variable, several roles | Split into several locals |
| Parameter used as a working variable | Local copy; never mutate the parameter |
| Type codes (`0/1/2`) | `enum` / typed id; different behaviour → polymorphism / strategy |
| Parallel arrays / "record" as loose fields | `record` / DTO class; rules in Service, not in the DTO |
| Bare collection with invariants enforced outside | Encapsulate (wrapper type / Api) |

### Statements / branching / loops

| Smell | Recommendation |
|---|---|
| Complex boolean in `if` | Named `bool` or bool method (`Is` / `Can` / `Has`) |
| Duplicate branches / identical tails | Consolidate; extract the common part |
| Deep `if / else` | Guard / early exit; typed exception on an invalid command |
| `switch` / ladder on type with logic in branches | Polymorphism, table (`Dictionary` / SO), `switch` expression as a thin dispatcher |
| Loop with several purposes | One purpose; simplify the condition; extract the body |
| Null checks everywhere | Explicit optional / `Try`; do not swallow query / command errors |

### Methods

| Smell | Recommendation |
|---|---|
| Long / several tasks | Extract method (Rider); keep cohesion |
| Wrapper without abstraction | Inline method |
| Near-identical methods | Parameterize / shared stateless Helper |
| Query changes state | Separate command and query (exceptions.md) |
| Many loose parameters | `record` / DTO / options; ~≤7 |
| Method knows others' internals / globals | Through DI / ports; Api, not View details |
| Init + I/O + computation mixed | Split across methods / layers |

### Classes / types / layers

| Smell | Recommendation |
|---|---|
| God class / mixed responsibilities | Extract class; check the layer (Core vs ViewComponents vs Input) |
| Inheritance "for code reuse" | Composition / delegation |
| Logic in DTO / View / Provider | Move into Service |
| View → Service directly | Through InputHandler |
| ViewComponents mutates Model | Only Service changes state |
| Duplication across features | Shared via Api / Helper per architecture.md |
| Blurred public Api | Narrow the contract |

## Report format

1. **Scope** — file list
2. **Verdict** — no refactoring needed / has recommendations / many smells
3. Each recommendation: `Severity (Major / Minor / Nit)` — `File:line` — smell — action — why — risk on apply
4. No smells → say «рефакторинг не требуется» explicitly
5. At the end (if there are Major / Minor): short apply order; implement only after ok
