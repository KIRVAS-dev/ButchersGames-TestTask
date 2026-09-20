---
paths:
  - "ButchersGames/Assets/_Project/Scripts/**/*.cs"
  - "**/*.unity"
  - "**/*.prefab"
---

# Unity MCP

Когда читать: правки `ButchersGames/Assets/_Project/Scripts/**/*.cs`, сцены, `**/*.prefab`.

Проект: `ButchersGames/`. Скрипты: `Assets/_Project/Scripts/`. Сцены: `Assets/_Project/Scenes/` (Bootstrap, Core).

Server: `unityMCP`, tools — `mcp__unityMCP__*`. Перед вызовом непривычного тула — свериться со схемой тула, не угадывать аргументы. C# rename/format — **не здесь**; см. [rider-mcp.md](rider-mcp.md). Архитектура — [architecture.md](architecture.md).

## Обязательно через Unity MCP

| Задача | Tool | Зачем MCP |
|---|---|---|
| Сцена (load/save/hierarchy/selection) | `mcp__unityMCP__manage_scene`, `mcp__unityMCP__find_gameobjects` | бинарный/YAML scene state |
| GameObject / компоненты / свойства | `mcp__unityMCP__manage_gameobject`, `mcp__unityMCP__manage_components` | Inspector-эквивалент |
| Prefab create/edit/apply | `mcp__unityMCP__manage_prefabs` | не править `.prefab` «вслепую» |
| Play mode / editor state | `mcp__unityMCP__manage_editor` | контроль play mode |
| Console после скриптов / domain reload | `mcp__unityMCP__read_console` | проверка compile errors |
| Asset search (shader, prefab, и т.п.) | `mcp__unityMCP__manage_asset` | поиск ассетов |

## Запрещено

- Большой ручной diff / переписывание `.unity`, `.prefab`, `.asset` вместо Unity MCP
- Угадывать содержимое сцены или hierarchy без запроса к MCP
- Продолжать после правок скриптов без проверки console на compile errors
- Подменять Unity MCP «креативным» YAML, если MCP доступен

## Must после C#

1. `reformat_file` (Rider) — один вызов за задачу, [rider-mcp.md](rider-mcp.md); format меняет файлы и может вызвать повторную компиляцию, поэтому он **до** refresh
2. `mcp__unityMCP__refresh_unity` (`compile`, `wait_for_ready`) — дождаться конца компиляции (`isCompiling` в состоянии editor)
3. `mcp__unityMCP__read_console` — Errors; исправить до шага, зависящего от новых типов
4. При необходимости — EditMode-тесты: `mcp__unityMCP__run_tests` (при открытом Editor) или `unity test` (только при закрытом)
5. Новые компоненты / play mode — только после чистой компиляции

## Инстанс

Несколько Unity → `mcp__unityMCP__set_active_instance` с точным `Name@hash` до остальных вызовов.

## Последовательности

**A. Осмотреть сцену** — `manage_scene` (active/load) → hierarchy (`page_size` ~50, paging) → find target → `manage_components` с `include_properties=false` сначала.

**B. Изменить объект** — find target → `manage_gameobject` / `manage_components` → при пачке `batch_execute` → save сцены при необходимости → `read_console`.

**C. Prefab** — `manage_prefabs` / `manage_asset` → правки через MCP (не YAML) → apply/save → `read_console`.

**D. После C#** — `reformat_file` → `refresh_unity` (`isCompiling == false`) → `read_console` Errors → при необходимости тесты → затем компоненты / play.

**E. Play mode** — `manage_editor` enter → проверка → exit перед структурными правками сцены/ассетов (если требует editor) → `read_console`.

## Payload / токены

- Hierarchy и components — paging, summary-first
- `include_properties=true` — только когда нужны значения
- Asset search — скромный `page_size`; `generate_preview=false` по умолчанию
- Однотипные мутации — `mcp__unityMCP__batch_execute`

## Tools (ориентир)

| Область | Tools |
|---|---|
| Сцена | `manage_scene` |
| Объекты | `manage_gameobject`, `find_gameobjects` |
| Компоненты | `manage_components` |
| Prefab / assets | `manage_prefabs`, `manage_asset` |
| Editor / play | `manage_editor` |
| Console | `read_console` |
| Пачки | `batch_execute` |
| Инстанс | `set_active_instance` |

(Полные имена — с префиксом `mcp__unityMCP__`.) Перед вызовом непривычного тула — свериться со схемой, не угадывать аргументы.

## Coplay MCP vs Pipeline (`unity command eval`)

В проекте доступны два независимых моста к Editor: **Coplay MCP** (`mcp__unityMCP__*`, описан выше; включает `mcp__unityMCP__execute_code`) и **Unity CLI + `com.unity.pipeline`** (пакет `0.7.0-exp.1` есть в `ButchersGames/Packages/manifest.json`; `unity command eval` — произвольный C# в живом Editor, порт 7800 по умолчанию). Транспорты разные и не конфликтуют, но выбор между ними — «какой инструмент подходит задаче», а не «какой подключён». Перед первым использованием CLI в сессии — `unity status` (состояние `ready`, нужный проект); `unity pipeline list` — если Editor не отвечает (Safe Mode из-за ошибок компиляции).

**По умолчанию — типизированный тул.** Любая задача, которая укладывается в существующий `manage_*` / `find_gameobjects` / `read_console` / `batch_execute`, — только через него: у тула фиксированная схема параметров, вызов ревьюабелен (понятная строка для запроса одобрения — [../../CLAUDE.md](../../CLAUDE.md) §1.4), тул физически не может сделать больше, чем описано в схеме.

**`execute_code` / `unity command eval` — точечно, когда типизированного тула нет:**

1. Специфичный API движка / внутреннее состояние без готового `manage_*`-тула (пример: `SGG.PerfMeter.Editor.Mcp.PerfMeterMcpCommands.*` — [perfmeter.md](perfmeter.md))
2. Read-only диагностика, для которой нет своего тула
3. Разовый скрипт для узкой задачи, явно не покрываемой существующими тулами
4. Официальные `unity:*` skills, жёстко зашитые на `eval` — там выбора нет

**`unity test` / `unity build` — только при закрытом Editor.** При открытом Editor на этом проекте они падают: `already open in a running Editor`, код выхода 6 (проверено `unity test` на этом проекте). При открытом Editor — `mcp__unityMCP__run_tests` и `mcp__unityMCP__manage_build`. `unity run --command` переиспользует уже открытый Editor и оставляет его работать.

**Без проговаривания (read-only):** MCP — `read_console`, чтение сцены и hierarchy, `find_gameobjects`, ресурсы редактора; CLI — `unity status`, `unity logs`, `unity doctor`, `unity pipeline list`, `unity command` без имени (листинг).

**Не использовать `execute_code` / `eval`, если то же самое покрывает typed-тул.** Мутирующий `execute_code` — произвольный код с полным доступом к `UnityEditor.*`/`UnityEngine.*` без ограничений схемы; даже когда permission mode пропускает такой вызов без вопроса, проговорить его отдельно ([../../CLAUDE.md](../../CLAUDE.md) §1.2, §1.4) — в отличие от именованного `manage_*` с фиксированными параметрами, по коду заранее не видно, что именно он сделает.

Если `unity status` не видит Editor, хотя он открыт, — не подменять это скрытым обходом (например, отдельным headless-Editor): сказать пользователю и уточнить (возможные причины: Safe Mode, песочница агента).

## Если Unity MCP недоступен

Остановиться и сказать пользователю. Не эмулировать сцену точечным diff «наугад».

## Пути

Относительно `Assets/` (forward slashes), если tool не требует абсолютный путь.

## Разделение

| Задача | MCP |
|---|---|
| Сцена, prefab, components, play mode, console | **Unity** |
| Rename C# | **rider** `rename_refactoring` |
| Format C# | **rider** `reformat_file` |
