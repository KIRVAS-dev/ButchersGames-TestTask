---
paths:
  - "ButchersGames/Assets/_Project/Scripts/**/*.cs"
---

# Typed exceptions и Guard

Когда читать: `ButchersGames/Assets/_Project/Scripts/**/*.cs`.

Инфраструктура: `ExtendedException`, `Guard`, namespace `ExtendedExceptions`.

## База

```csharp
public class ExtendedException : Exception
{
    protected ExtendedException(string id, string message)
        : base(message: $"{id}: {message}") { }
}
```

- Message в консоли: `{id}: {text}`
- **id** — kebab-case, префикс фичи + порядковый номер с 1 в файле (`feature-1`, `feature-2`)
- Message **без** точки в конце
- `base(id, message)` — одна строка в конструкторе

## Организация

- Один файл `Exceptions.cs` на фичу (обычно `{Layer}/{Feature}/Api/`)
- Namespace фичи **без** суффикса `.Api`
- `sealed class … : ExtendedException` — `public`, если тип создают или ловят в другой сборке; иначе `internal` ([class-design.md](class-design.md))
- Отдельный файл на каждый тип — **не** создавать
- **Запрещено:** generic-исключение с произвольной строкой `reason`

## View / Inspector

MonoBehaviour, SerializeField, сцена.

| Ситуация | Тип | Конструктор |
|---|---|---|
| Не назначено поле | `Missing{Feature}FieldException` | `fieldName`, `objectName` |
| Невалидное число на MB | `Invalid{Feature}ValueException` | `fieldName`, `objectName`, `value` |

- Типы с authored wiring / config реализуют `IValidatable` (`ContentValidation` package); `Validate()` только проверки, без side effects
- **Когда вызывается:** session — `SessionValidation` из DI до `PrepareGame`; level content — после `LoadLevel` / `CurrentLevel.Set`; Editor — `Tools/ContentValidation` (collect-all)
- Init, которому нужны уже проверенные поля (пулы, словари, `TurnAnimator` и т.п.) — `IWarmupLifecycle.Warmup()` после session в `CoreEntryPoint`, не `Awake → Validate`
- View/MB: предпочитать `void IValidatable.Validate()` (explicit); `public void Validate()` — только если нужен вызов с concrete
- Ссылки: `Guard.AgainstNull` / `AgainstNullOrEmpty` + typed factory
- Числа: `Guard.AgainstNegative` / `AgainstNonPositive` / `AgainstLessThan` / … → `Invalid{Feature}ValueException`
- В message View допускается `gameObject.name` как `objectName`

## Core / domain

Model, Service, ScriptableObject-config без привязки к сцене.

| Ситуация | Тип | Конструктор |
|---|---|---|
| Невалидное значение в SO config | `Invalid{Feature}ValueException` | `fieldName`, `value` — **без** `objectName` |
| Домен / контекст | `Missing/Invalid{Feature}{Topic}Exception` | по смыслу фичи |

- Model **без** `UnityEngine.*`
- Config (owned SO): `IValidatable` через **`public void Validate()`** (не explicit) — владелец зовёт `_config.Validate()` после `Guard.AgainstNull`; session/Editor тоже через `IValidatable`. Explicit на config даёт лишний `((IValidatable)_config)` без выигрыша
- Политика soft-checks (запрет soft return / LogWarning вместо throw) — см. [architecture.md](architecture.md)

## Guard

- API: `Guard.Against*(…, Func<ExtendedException> exceptionFactory)`
- Factory ленивая — исключение создаётся только при нарушении
- Проверки через `Guard`, не через inline `if` + `throw` с сырым текстом

Типичные методы: `AgainstNull` (`object` и `UnityEngine.Object`), `AgainstNullOrEmpty`, `AgainstNegative`, `AgainstNonPositive`, `AgainstLessThan`, `AgainstGreaterThan`, `AgainstInvalidRange`, `AgainstTrue`.

## Локальная фабрика (call site)

При **≥2** проверках с **одним** типом исключения и одной формой аргументов — вынести factory в **локальную функцию** (не в `Func<…>`-переменную):

```csharp
Guard.AgainstNull(_a, () => Missing(nameof(_a)));
Guard.AgainstNull(_b, () => Missing(nameof(_b)));

return;

ExtendedException Missing(string fieldName) => new MissingFooFieldException(fieldName, gameObject.name);
```

- Локальная функция — в **конце** метода, после явного `return;`; имя в `PascalCase` (`Missing`, `Invalid`) — так требуют инспекции Rider («Use local function», «Local functions» naming, «Separate local function with explicit return»)
- `Func<string, ExtendedException> missing = …` не использовать
- Одиночный `Guard` — inline `() => new …`, без локальной функции
- Разные типы в одном методе — отдельная локальная функция на тип или inline
- `nameof` обязателен; `CallerArgumentExpression` в этот rule не входит
