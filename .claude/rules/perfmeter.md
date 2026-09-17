# PerfMeter

Когда читать: триггер `Perf` / `PerfMeter` / «перф» — в начале сообщения или отдельной строкой. Вне триггера — правило не применять.

Kit для **SGG PerfMeter** (`com.sungeargames.perfmeter`). Платформа, сцены, стек, целевой FPS — [architecture.md](architecture.md) / [../../CLAUDE.md](../../CLAUDE.md) §2. Unity Editor bridge — [unity-mcp.md](unity-mcp.md).

Интервью / grill — **не** здесь: при необходимости подготовка плана — через [planning.md](planning.md) (`grill me`), не дублировать.

## Доступ

- Отдельного MCP-сервера у PerfMeter **нет**
- Основной путь данных: **Unity MCP** → `mcp__unityMCP__execute_code` → `SGG.PerfMeter.Editor.Mcp.PerfMeterMcpCommands.*`
- Zero-code settings: `Assets/Resources/SGG.PerfMeter/perfmeter-settings.json`
- Setup UI: **SGG → Perfmeter → Setup** (тумблеры **Setup → Work mode**: Enabled / Auto Start / Collect Metrics / Show Overlay)

```csharp
return SGG.PerfMeter.Editor.Mcp.PerfMeterMcpCommands.SetupStatus();
return SGG.PerfMeter.Editor.Mcp.PerfMeterMcpCommands.RuntimeStatus();
return SGG.PerfMeter.Editor.Mcp.PerfMeterMcpCommands.MetricsLatest();
return SGG.PerfMeter.Editor.Mcp.PerfMeterMcpCommands.DeviceInfo();
return SGG.PerfMeter.Editor.Mcp.PerfMeterMcpCommands.AlertsLatest();
return SGG.PerfMeter.Editor.Mcp.PerfMeterMcpCommands.RuntimeModeSet("{\"mode\":\"Background\"}");
return SGG.PerfMeter.Editor.Mcp.PerfMeterMcpCommands.SessionStart("{\"warmup_seconds\":2,\"sample_interval_seconds\":0.25,\"max_samples\":240}");
return SGG.PerfMeter.Editor.Mcp.PerfMeterMcpCommands.SessionExport("{\"format\":\"json\",\"path\":\"Temp/PerfMeter/session.json\"}");
```

### Конфиг и Setup

- `perfmeter-settings.json` — живой источник дефолтов; пакет перечитывает его при работе сессии/алертов/overdraw
- **`setup.run` / мастер Setup перезаписывает JSON** своими дефолтами (часто `autoStart=true`, overlay on). После любого Setup — заново выставить baseline для замера (см. Pre-flight), не полагаться на файл «как был»
- Непустой `overlayPresets` + пустой `activeOverlayPresetId` → фолбэк на первый preset оверлея

## Слои метрик и среды

| Слой | Примеры | Где надёжен |
|---|---|---|
| Frame Timing | FPS, CPU/GPU frame time | Editor, Dev Build, player (если Frame Timing Stats on) |
| ProfilerRecorder | draw calls, SRP, memory, GC, overdraw counters | **Editor / Development Build**; в обычном release часто `unavailable` |

Для этого проекта:

- **Дефолт замера:** Editor Play Mode на целевой gameplay-сцене (Core, [architecture.md](architecture.md)) + Unity MCP
- Editor / Dev Build → counts и **относительные** сравнения (A/B); абсолютные мс для целевой платформы (WebGL) — **с оговоркой**
- Итоговый вердикт для ship/fix подтверждать на **целевом player** (WebGL build), когда цифры решают

## Гипотеза → preset (без полного grill)

Если пользователь назвал узкое место — выбрать узкий preset/модули; иначе baseline + широкий glance:

| Гипотеза | Preset / фокус |
|---|---|
| Общий baseline / «просто сними» | Background, overlay off; потом Timing glance |
| CPU-bound | Timing |
| Draw calls / batching | Rendering (+ SrpBatcher) |
| Память / GC | Memory |
| Overdraw | короткий Overdraw/heatmap **только** diagnostic-окно, не baseline |

Выбор оверлея влияет на UI/шум DC; на сбор session samples — слабо. Для чистого baseline оверлей **выключен**.

## Pre-flight

1. Unity MCP подключён ([unity-mcp.md](unity-mcp.md)); Play Mode на целевой gameplay-сцене (Core)
2. `SetupStatus` — Frame Timing Stats on; JSON loaded если zero-code; URP Render Graph feature на **активном** renderer (после правок URP — перепроверить)
3. **Нормализация baseline:** collection **Background**; overlay **hidden** (или Minimal только если нужен human glance); FMOD Debug / прочий debug UI off — они раздувают Draw Calls
4. Editor **focused**, не paused; иначе Frame Timing может быть `NotCollected`
5. `OverdrawDiagnostic` / heatmap — только короткие diagnostic-окна, не baseline
6. Среда в отчёте явно: Editor ≠ целевой player (WebGL) ≠ device
7. Пакет не установлен / handlers не компилируются → остановиться и сказать; не парсить скриншоты overlay вместо JSON

## Процедура (Editor + MCP — основной путь)

1. `RuntimeEnsure` / `RuntimeStatus`
2. При необходимости `RuntimeResetStats`; warmup **≥2 с** (или `warmup_seconds` в `SessionStart`)
3. Снять: `MetricsLatest`, `DeviceInfo`, `AlertsLatest`; опционально camera / rendergraph snapshot
4. A/B или артефакт: `SessionStart` → сценарий → `SessionStop` / `SessionExport`
   - Под длительность подобрать `sample_interval_seconds` и `max_samples` (пример: 240 × 0.25 с ≈ 1 мин; для длиннее — поднять interval или maxSamples)
5. Сопоставить метрики со сценой через Unity MCP (renderer/UI counts и т.п.), не гадать
6. Lifecycle-алерты на старте (`fps.below_target` и т.п.) не смешивать со steady-state без warmup

Запись сессии в Editor агент стартует/останавливает **сам** через MCP. Отдельная экранная кнопка для этого пути не нужна.

## Ручной брекет в player (опционально)

Только по явной фразе: «ручной замер на устройстве» / «кнопка» / «брекет в билде» (и аналоги). Обычный `Perf` этот путь **не** стартует.

Нужен, когда MCP в рантайме нет (Development / целевой WebGL player) и окно замера задаёт пользователь тапом START/STOP.

**Источник кода (не выдумывать):** `.cursor/prompts/perfmeter/PerfMarkDebugButton.cs.txt`
Шаблон вне `Assets/` — Unity его не импортирует. В билде кнопки нет, пока копию не положили в проект и не собрали player.

### Цикл

1. Скопировать шаблон → `Template-UnityProject/Assets/Debug/PerfMark/PerfMarkDebugButton.cs` (папку создать при необходимости). Код не генерировать с нуля.
2. Дождаться компиляции; `read_console` — без Errors ([unity-mcp.md](unity-mcp.md)).
3. Собрать **Development Build** WebGL. Обычный release: кнопка и glue под `#if DEVELOPMENT_BUILD || UNITY_EDITOR || PERFMETER_RELEASE` в ноль; для сознательного release-профайла — define `PERFMETER_RELEASE` до билда.
4. Запустить player. Скрипт сам создаёт overlay-кнопку (`RuntimeInitializeOnLoadMethod`) — сцену/префаб не трогать.
5. Пользователь: START → играет отрезок → STOP. В лог: `[PERFMARK]` (+ `[TEMP:perfmark]`). JSON: `Application.persistentDataPath/PerfMarks/session_n.json` (точный `path` — в STOP-маркере).
6. Забрать экспорт: WebGL — путь платформенный (browser storage); спросить у пользователя файл/способ выгрузки, не предполагать adb.
7. Разобрать JSON + маркеры; отчёт как ниже. Canvas кнопки раздувает DC — не смешивать с «чистым» baseline без оговорки.
8. **Cleanup (обязательно):** удалить `Assets/Debug/PerfMark/PerfMarkDebugButton.cs` (+ `.meta` и пустую папку); grep по репо — `[TEMP:perfmark]` и `PerfMarkDebugButton` не остались. Шаблон в `.cursor/prompts/` **не** удалять.

Кнопку в `Assets/` между замерами не держать. Editor+MCP baseline — без кнопки.

## Как читать метрики

| Сигнал | Поля |
|---|---|
| Бюджет | `target_fps` / `frame_budget_ms` (60 → 16.67 ms) |
| FPS | `average_fps`, `one_percent_low_fps`, `point_one_percent_low_fps`, spikes |
| Bottleneck | `bottleneck`: CPU main / render / GPU / present/VSync / Balanced / unknown |
| Timings | `cpu_*_frame_time_ms`, `gpu_frame_time_ms` (если available) |
| Rendering | `draw_calls`, `set_pass_calls`, `vertices`, `srp_batcher_instances` |
| Memory | `system_used_memory_bytes`, `gc_reserved_memory_bytes`, `gpu_memory_bytes` |

### Ловушки

- **`batches = 0`** — classic Dynamic/Static/Instanced; при выключенном Dynamic Batching это норма. SRP Batcher → `srp_batcher_instances`
- Высокий Draw Calls при ~1:1 Sprite/Mesh/UI — слабый батчинг / материалы / много Canvas
- Unfocused / paused Editor → Frame Timing может быть `NotCollected`
- После Setup JSON мог сброситься — сверить overlay/mode перед цифрами

## Формат отчёта

- Вердикт **первым**: в бюджете / риски / вне бюджета (целевой FPS / WebGL — из запроса или контекста проекта)
- Среда: Editor / целевой player / device, сцена, collection mode, overlay on/off, focused
- Таблица: FPS, 1%/0.1% low, bottleneck, CPU/GPU ms, DC, SetPass, SrpInst, memory
- Findings: `Severity (Blocker / Major / Minor / Nit)` — область/метрика — суть — обоснование — рекомендация
- Только обоснованные замечания; **без правок кода**, пока пользователь не попросит
- Идеи оптимизаций — отдельным списком предложений

## Не делать

- Парсить скриншоты overlay / Console, если доступен MCP JSON
- Включать Overdraw heatmap для baseline
- Выдавать Editor-замер за целевой player / device без оговорки
- Править код «заодно» вне запроса
- Дублировать короткий perf-чеклист из §4 [../../CLAUDE.md](../../CLAUDE.md) — здесь полный runtime-замер
- Дублировать `grill-me` / planning footer из [planning.md](planning.md)
- Ставить TEMP-кнопку в `Assets/` без явного запроса на ручной брекет в player
- Генерировать код кнопки с нуля — только копия из `.cursor/prompts/perfmeter/PerfMarkDebugButton.cs.txt`

## Промпт для нового чата (копировать)

```
Perf

Сцена: Core (gameplay). Play Mode. Цель: целевой FPS / WebGL.

Сними метрики SGG PerfMeter через Unity MCP (Background, overlay off),
прогрей ≥2 с, прочитай metrics/status/device/alerts.
Отчёт: вердикт по бюджету, таблица метрик, findings с severity,
оговорка Editor≠целевой player. Без правок кода, пока не попрошу.
```
