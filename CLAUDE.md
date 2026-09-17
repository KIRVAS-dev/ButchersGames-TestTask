# WebGL-Template — правила для Claude Code

Адаптация Cursor-rules (`.cursor/rules/*.mdc`) под Claude Code. Этот файл загружается **всегда и целиком** — здесь только то, что должно действовать в любой момент (approval, архитектура-суть, principles, review-процедура, индекс). Триггерные / условные части (кодстайл, exceptions, MCP-тулинг, planning, perfmeter, полная архитектура) вынесены в `.claude/rules/*.md` — Claude читает нужный файл сам, когда открыт/правится `.cs` или сработал текстовый триггер, как описано в §5 «Индекс» ниже.

`.cursor/rules/*.mdc` (правила для Cursor) — отдельный, независимый набор; не читать, не синхронизировать и не трогать при правках `.claude/rules/*.md` или этого файла.

---

## 1. User approval mode (всегда)

Мутирующие тулы (Edit, Write, Bash, MCP-мутации) гейтит **сам Claude Code** через permission mode, который выбрал пользователь в интерфейсе сессии — отдельная текстовая грамматика триггеров поверх этого здесь не нужна и не может быть надёжной: харнесс одобрит или переспросит вызов согласно текущему permission mode независимо от того, что написано в этом файле, а правило, обещающее б**о**льшую строгость, чем реальный permission mode сессии, будет просто неверным в auto-accept режиме.

Ниже — то, что permission mode **не** решает: контент-политика (что агенту стоит предлагать и в каком объёме), а не разрешение на вызов конкретного тула.

### 1.1 Report-only независимо от permission mode

Триггер `Review` / «ревью» / `review`, `Refactor` / `refactor` / «рефактор» / «рефакторинг», «анализ», «обоснуй», «сравни», `audit`, «проверь» (без «исправь») — в начале сообщения или отдельной строкой: ответ **только** отчёт/анализ, без вызова мутирующих тулов, даже если permission mode сессии это разрешил бы. Правки — отдельным явным сообщением («исправь», «примени»).

Процедура ревью — §4 ниже. Процедура рефакторинга (анализ, каталог рекомендаций, формат отчёта) — [refactor.md](.claude/rules/refactor.md), читать целиком по триггеру.

### 1.2 Границы scope (не про разрешение вызова, про его объём)

Permission mode решает, можно ли вообще позвать Edit/Write/Bash/MCP-тул. Он не знает, что пользователь имел в виду под «этой задачей» — это остаётся на агенте:

- Не расширять правки за пределы названных файлов / фичи / слоя без явного нового запроса — **scope creep запрещён** даже если харнесс пропустил бы более широкую правку
- Новая формулировка задачи в сообщении не меняет уже согласованный план сама по себе — переспросить при неоднозначности
- `CLAUDE.md`, `.claude/rules/*`, `.cursor/rules/*` — трогать только когда это явно часть задачи, а не побочный эффект другой правки
- git push / force-операции / отправка сообщений вовне (issue, PR, сообщения) — проговорить явно перед вызовом, даже если permission mode пропустит без вопроса; общие причины — см. системные правила "Executing actions with care"

Наоборот, **входит** в scope согласованной правки `.cs` без отдельного переспроса: rename/format после каждой правки ([rider-mcp.md](.claude/rules/rider-mcp.md)), проверка компиляции / console после C# ([unity-mcp.md](.claude/rules/unity-mcp.md)) — это часть самой правки, а не отдельное действие.

### 1.3 План (Plan mode / grill)

Grill-цикл и структура плана — [planning.md](.claude/rules/planning.md). Implement начинается только после явного согласия на план: `EnterPlanMode` → обсуждение → `ExitPlanMode`, либо словесное «го» / «ок» / «да» **без новой задачи** в том же сообщении (это одобрение уже согласованного плана, а не карт-бланш на всё остальное — новый scope в том же сообщении требует отдельного согласования по §1.2).

### 1.4 Формат предложений, когда стоит объяснить, а не сразу делать

По умолчанию (кратко):

1. **Суть** — 1–2 предложения
2. **План** — 1–3 пункта
3. **Затронутые файлы / системы**
4. Если действие и так пройдёт через permission-prompt харнесса — не дублировать его отдельным вопросом в чате; если нет (крупная/неоднозначная задача даже в разрешающем permission mode) — спросить явно и остановиться до ответа

По триггерам («обоснуй», «подробно», «расскажи подробнее»):

1. Детальная гипотеза и риски
2. Варианты решения (плюсы / минусы)
3. Точный список изменений (файлы, команды)
4. План отката
5. Запрос одобрения

---

## 2. Architecture (суть, всегда)

Полная версия — [architecture.md](.claude/rules/architecture.md) (asmdef-граф, DI-регистрация, эталон фичи, роли Model/View, soft-checks). Craft/SOLID — §3 ниже. Codestyle — [codestyle.md](.claude/rules/codestyle.md). Exceptions — [exceptions.md](.claude/rules/exceptions.md). Unity — [unity-mcp.md](.claude/rules/unity-mcp.md). Rider — [rider-mcp.md](.claude/rules/rider-mcp.md).

### Context

- Платформа: **Unity 6.5**, WebGL playable ad
- Скрипты: `Template-UnityProject/Assets/_Project/Scripts/`
- OOP, **не ECS**

| Сцена | Назначение |
|---|---|
| **Bootstrap** | `ProjectScope`, загрузка геймплей-сцены |
| **Core** | Gameplay: scopes фич, View, Providers |

### Stack

VContainer (DI), UniTask (async), R3 (реактивное связывание Model → View), DOTween/FMOD (анимации и звук во View). Unity API сверять с документацией Unity 6.5.

### Слои (кратко)

`ExtendedExceptions` ← `Core` ← `ViewComponents`; `Core` ← `Core.Input` ← `Bootstrap` ← `Infrastructure.Bootstrap`; `Core` ← `Infrastructure.Persistence` ← `Bootstrap`; `Input` ← `Core.Input`; `Rendering` изолирован (URP only). Config SO — во ViewComponents (`I*Settings` в Core). Полный asmdef-граф — [architecture.md](.claude/rules/architecture.md).

### Dependency rule (топ)

| Запрещено | Почему |
|---|---|
| `Core` → `ViewComponents` | Core не знает сцену |
| `Core.Input` → `ViewComponents` | Handler зависит только от портов Core и низкого Input |
| View вызывает `I*Service` напрямую | Обход входного адаптера |
| Model использует `UnityEngine.*` | State должен быть pure C# |
| View принимает gameplay-решения / меняет Model | Логика только в Service |

Разрешено: `ViewComponents` → `Core` (реализует `I*View` / `I*Provider`); `Core.Input` → `Core` + `Input`; Bootstrap регистрирует конкретные View в DI.

### Decision tree

```
Пользовательский ввод? → InputHandler → I*Service
Gameplay state / правила? → Model + Service (Core)
Отображение / анимация / VFX? → ViewComponents View
Данные сцены для Core? → Provider (ViewComponents → Api)
DI / bootstrap / load scene? → ProjectScope / CoreScope
URP / post-effects? → Rendering
```

**Anti-patterns:** View → Service напрямую; Model с `UnityEngine.*`; бизнес-логика во View; Core → ViewComponents.

### Runtime flow

```
Bootstrap (ProjectScope) → additive load Core.unity
  → CoreScope → CoreEntryPoint.StartListening() (InputHandlers)
  → gameplay → dispose handlers on CoreEntryPoint.Dispose()
```

Ключевые типы: Infrastructure `EntryPoint`, `CoreScope`, `CoreEntryPoint`, scene loader API.

### Soft-checks

Bad data и невалидная конфигурация → `ExtendedException` (обычно через `Guard` — см. [exceptions.md](.claude/rules/exceptions.md)). Запрещено: soft `return`/early-exit без throw, `LogWarning` вместо исключения, «проглотить ошибку и продолжить» в Service/Validate/Provider. UX-гейты во InputHandler (busy → игнор клика) — не soft-check.

**Не предлагать:** ECS, System Groups, Event Queue, Command Bus, CQRS, Presenter на каждую сущность.

**MCP:** Unity/Rider — global `~/.cursor/mcp.json` (Claude Code — MCP-серверы сессии, см. [unity-mcp.md](.claude/rules/unity-mcp.md) / [rider-mcp.md](.claude/rules/rider-mcp.md)); project `.cursor/mcp.json` не используется.

---

## 3. Principles (всегда)

Hub. Детали — по таблице; не дублировать codestyle / exceptions / design здесь.

| Задача | Rule |
|---|---|
| Слои, decision tree, dependency | §2 выше |
| asmdef, DI, feature slice, эталон фичи | [architecture.md](.claude/rules/architecture.md) |
| Классы | [class-design.md](.claude/rules/class-design.md) |
| Методы | [method-design.md](.claude/rules/method-design.md) |
| Переменные | [variable-design.md](.claude/rules/variable-design.md) |
| Формат, naming, async | [codestyle.md](.claude/rules/codestyle.md) |
| Typed exceptions | [exceptions.md](.claude/rules/exceptions.md) |
| Unity сцена/prefab | [unity-mcp.md](.claude/rules/unity-mcp.md) |
| C# rename/format | [rider-mcp.md](.claude/rules/rider-mcp.md) |
| План / grill | [planning.md](.claude/rules/planning.md) |
| Ревью | §4 ниже |
| Рефакторинг (анализ) | [refactor.md](.claude/rules/refactor.md), по триггеру |
| Runtime perf | [perfmeter.md](.claude/rules/perfmeter.md), по триггеру `Perf` |
| Одобрение мутаций | §1 выше |

Общие практики написания кода, **без** привязки к языку, стеку и слоям. Конкретика проекта — §2 выше, [codestyle.md](.claude/rules/codestyle.md), [exceptions.md](.claude/rules/exceptions.md). Не дублировать эти файлы здесь.

### Читаемость

Код должен быть понятен из имён и структуры. «Умные» трюки без выигрыша в ясности — не цель.

### SOLID (кратко)

Finding только при реальном нарушении в scope.

| | Смысл | Finding, когда |
|---|---|---|
| **S** | Один повод менять тип | Тип смешивает несвязанные ответственности |
| **O** | Расширение без ломки существующего | Новый кейс требует править ядро вместо расширения |
| **L** | Подтип не ломает контракт базы | Подмена типа меняет ожидаемую семантику |
| **I** | Узкие контракты | Клиента заставляют реализовывать неиспользуемое |
| **D** | Зависимость от абстракций на границах | Высокоуровневое жёстко завязано на детали, где нужен порт |

### DRY / YAGNI / KISS

Повторяющиеся **стабильные** блоки — выносить. Не плодить абстракции «на будущее» (YAGNI-finding — лишний слой без второго потребителя). Проще достаточное решение предпочтительнее общего framework'а под одну задачу.

### Композиция

Предпочитать композицию наследованию, если иерархия не даёт явного выигрыша в модели.

### Поток данных и зависимости

Соблюдать направление, заданное в §2 / [architecture.md](.claude/rules/architecture.md) (слои, порты, односторонний bind UI ← state). Finding — обратный вызов через слой, протечка деталей вверх/вниз, бизнес-решения во view-слое.

### Hot path

Не выполнять дорогие операции на частом пути (Update / часто вызываемый код): поиск по сцене, повторные тяжёлые запросы компонентов, лишние аллокации. Finding — только если реально на hot path.

### Не делать

Рефакторить или замечания «ради SOLID / чистоты» без нарушения rules, бага или измеримой деградации. Не дублировать сюда Unity API, naming C#, soft-checks, шаблоны Exception.

---

## 4. Review (всегда — процедура; триггер `Review`/«ревью» — для применения)

Триггер: `Review` / «ревью» / `review` — в начале сообщения или отдельной строкой. Вне триггера — не применять.

Rules: §2, §3, [class-design.md](.claude/rules/class-design.md), [method-design.md](.claude/rules/method-design.md), [variable-design.md](.claude/rules/variable-design.md), [codestyle.md](.claude/rules/codestyle.md), [exceptions.md](.claude/rules/exceptions.md) (+ делегаты). Правки — §1.

### Scope

- По умолчанию — весь проект (`Template-UnityProject/Assets/_Project/Scripts/` по слоям)
- Пользователь задал scope — ревью **только в нём**; не расширять самостоятельно

### Процедура

1. Определить scope. Для коммита/диапазона — `git log` / `git show`. Файлы в scope — **целиком**, не только диф.
2. Проверки по категориям:
   - **Архитектура** — §2, детали — [architecture.md](.claude/rules/architecture.md)
   - **Principles** — §3
   - **Class design** — АТД, интерфейс, инкапсуляция, наследование vs композиция — [class-design.md](.claude/rules/class-design.md)
   - **Method design** — одна задача, имена, параметры, return vs void — [method-design.md](.claude/rules/method-design.md)
   - **Variable design** — init, scope, binding, одна цель — [variable-design.md](.claude/rules/variable-design.md)
   - **Кодстайл** — [codestyle.md](.claude/rules/codestyle.md)
   - **Исключения / Guard / Validate** — [exceptions.md](.claude/rules/exceptions.md); soft-checks — §2
   - **Баги и логика:** null, async/`CancellationToken`, утечки подписок/tween, edge cases
   - **Оптимизация (короткий чеклист):** аллокации в hot path, `GetComponent`/`Find` в частом коде, лишние обновления — согласовано с §3; сомнение в API/perf → docs/Context7, не гадать. Полный runtime-замер — [perfmeter.md](.claude/rules/perfmeter.md), по триггеру `Perf`
   - **C# / engine API:** актуальные под Unity 6.5, без Deprecated
3. **Только отчёт**; правки — по отдельному ok

### Findings

- Вердикт **первым**: соответствует / есть замечания / блокеры
- `Severity (Blocker / Major / Minor / Nit)` — `Файл:строка` — суть — обоснование — рекомендация
- Нет findings → явно «без замечаний»
- Не выдумывать советы; вкусовщина — не finding

---

## 5. Индекс reference-файлов (`.claude/rules/`)

Эти файлы **не загружаются автоматически** — читай нужный, когда открыт/правится `.cs` в `Scripts/**` или сработал триггер, как описано в колонке «Когда».

| Файл | Когда |
|---|---|
| [architecture.md](.claude/rules/architecture.md) | Правки `Scripts/**/*.cs`; вопросы про слои/DI/эталон фичи |
| [codestyle.md](.claude/rules/codestyle.md) | Правки `Scripts/**/*.cs` |
| [exceptions.md](.claude/rules/exceptions.md) | Правки `Scripts/**/*.cs` |
| [rider-mcp.md](.claude/rules/rider-mcp.md) | Правки `Scripts/**/*.cs` (rename/format) |
| [class-design.md](.claude/rules/class-design.md) | Правки `Scripts/**/*.cs` |
| [method-design.md](.claude/rules/method-design.md) | Правки `Scripts/**/*.cs` |
| [variable-design.md](.claude/rules/variable-design.md) | Правки `Scripts/**/*.cs` |
| [unity-mcp.md](.claude/rules/unity-mcp.md) | Scripts, сцена, prefab |
| [planning.md](.claude/rules/planning.md) | Триггер: `grill me` / «план» / Plan mode |
| [refactor.md](.claude/rules/refactor.md) | Триггер: `Refactor` / «рефактор» / «рефакторинг» |
| [perfmeter.md](.claude/rules/perfmeter.md) | Триггер: `Perf` / `PerfMeter` / «перф» |

Без открытого `.cs`: [architecture.md](.claude/rules/architecture.md) или открыть файл в Scripts.
