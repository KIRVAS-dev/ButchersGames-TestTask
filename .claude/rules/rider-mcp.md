# Rider MCP

Когда читать: правки `.cs` в `ButchersGames/Assets/_Project/Scripts/**`.

Rename **и** Format — один MCP-сервер `rider` (официальный JetBrains Rider MCP): rename — `mcp__rider__rename_refactoring`; format — **в этом rule только** `mcp__rider__reformat_file`; остальные tools сервера `rider` вне scope этого файла.

Сцена / prefab / console — **не здесь**; см. [unity-mcp.md](unity-mcp.md). Стиль — [codestyle.md](codestyle.md).

## Rename

Переименование C#-символов (тип, метод, свойство, поле, параметр и т.д.) — **только** через `mcp__rider__rename_refactoring`.

**Запрещено** для rename:

- `StrReplace` / search-replace по проекту
- переписывание файла с новым именем типа
- `git mv` / ручное переименование `.cs` вместо Rider, если файл связан с типом

| Аргумент | Значение |
|---|---|
| `filePath` | путь к `.cs`, где объявлен символ (абсолютный или относительно solution) |
| `symbolName` | имя символа: `TypeName`, `MemberName` или `TypeName.MemberName` |
| `newName` | новое имя |
| `rootFolder` | корень Unity-проекта, где лежит `.sln` — передавать всегда, когда известен |

При сомнении в целевом символе / на публичном Api — сначала вызвать с `preview: true` и свериться с `affects`, затем вызвать без `preview` для применения. `preview` **не** запускает conflict-анализ — чистый preview всё равно может дать `ok=false` на apply.

`ok=false` с `conflicts` — rename отклонён, файлы не тронуты; скорректировать `newName`, не обходить точечным diff. `error.kind` (`new_name_invalid`, `no_renamable_symbol`, `new_name_matches_current`, `symbol_invalidated`, …) — типовые причины, не гадать.

Rider запущен, solution открыт. Bridge недоступен → **остановиться и сказать пользователю**; не подменять rename точечным diff.

После успеха Rider обновляет ссылки сам (`touched` в ответе). Ручной проход по references — только при ошибке / частичном результате tool.

## Format

После **каждого** изменения `.cs` (создание или правка) — `mcp__rider__reformat_file`. При нескольких файлах — после каждого файла (или одним вызовом со списком `files`).

Пути в `files` — **относительно корня solution**, не абсолютные.

**Запрещено** переписывать файл целиком ради пробелов, переносов или порядка членов.

Rider / `reformat_file` недоступен → **остановиться и сказать пользователю**; не подменять format ручным diff и не считать шаг завершённым.

## Исключения

| Область | Инструмент |
|---|---|
| `.unity`, `.prefab`, `.asset`, `.asmdef` | Unity MCP ([unity-mcp.md](unity-mcp.md)) или точечный diff по задаче |
| Новый файл с новым типом (не rename) | создать файл; сразу `reformat_file` |
| Конфиги вне `Assets/` | Rider не требуется |

## Разделение

| Задача | MCP |
|---|---|
| Rename C#-символов | **rider** `rename_refactoring` |
| Format C# | **rider** `reformat_file` |
| Сцена, prefab, components, play mode, console | **Unity** ([unity-mcp.md](unity-mcp.md)) |
