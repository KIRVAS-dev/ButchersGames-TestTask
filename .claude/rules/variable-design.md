---
paths:
  - "ButchersGames/Assets/_Project/Scripts/**/*.cs"
---

# Variable design

Когда читать: `ButchersGames/Assets/_Project/Scripts/**/*.cs`.

Init, scope, lifetime, binding. Casing — [codestyle.md](codestyle.md); параметры — [method-design.md](method-design.md); поля — [class-design.md](class-design.md); конфиги/DI — [architecture.md](architecture.md).

## Объявление и инициализация

- Объявлять переменную **как можно ближе** к первому использованию
- Инициализировать **рядом** с использованием; счётчики/аккумуляторы цикла — непосредственно перед циклом (или в `for`)
- Повторно используемый код — проверить, нужна ли **повторная** инициализация внутри повтора
- Поля класса — в конструкторе / field initializer / DI; `[SerializeField]` — источник для ViewComponents ([codestyle.md](codestyle.md))
- Не использовать значение «с прошлой жизни» — C# definite assignment для locals; для полей и коллекций не полагаться на удачу
- Не вводить голые глобалы «для удобства»

Finding: init в начале метода, а использование через десятки строк; счётчик не сброшен при повторном проходе.

## Область видимости и время жизни

- Минимально достаточная видимость: local → private field → шире только по нужде
- Короче **время жизни** и плотнее обращения к переменной
- Группировать чтения/записи одной переменной; не растягивать использование через весь метод без нужды
- Зависимости через DI/порты ([architecture.md](architecture.md)), не static/mutable shared без контракта
- `using` / короткий scope для disposable и временных ресурсов

Finding: поле класса вместо local; переменная живёт от начала метода до конца, хотя нужна на 5 строк.

## Время связывания (гибкость vs сложность)

Давать гибкость **по требованию**, не про запас (YAGNI — [../../CLAUDE.md](../../CLAUDE.md) §3).

Практический порядок (от раннего к позднему):

1. литерал / магическое число — **избегать** ([codestyle.md](codestyle.md))
2. `const` / `static readonly` / именованная константа
3. Config / ScriptableObject / данные с Provider ([architecture.md](architecture.md))
4. значение при создании объекта / из DI
5. чтение по требованию в runtime — только если реально нужна смена без пересборки

Finding: «голые» литералы по коду; runtime-конфиг на всё подряд без требования.

## Одна цель на переменную

- Одна переменная — **одна роль**; не переиспользовать `temp`/`x` для несвязанных задач
- Не смешивать смыслы в одном значении (sentinel: «счётчик, но −1 = ошибка») — лучше раздельные типы/`bool`/nullable/`Try`/typed exception
- Параметры тоже одноцелевые ([method-design.md](method-design.md))
- Сложное булево условие → локальный `bool` с именем смысла (`isReadyToSave`)

Finding: `temp` сначала для одного, потом для другого; код статуса, зашитый в то же поле, что и данные.

## Данные и управляющие структуры (эвристика)

- последовательность полей/шагов → последовательность операторов
- «ровно один из вариантов» → `if` / `switch` / полиморфизм
- повтор однотипных элементов → цикл / `foreach` ([codestyle.md](codestyle.md))
- много однотипных ветвлений → table / стратегия ([method-design.md](method-design.md))

Finding: ручной разворот итеративных данных без цикла; гигантский `switch` там, где нужен тип/стратегия.

## SerializeField и Inspector

- Inspector / hierarchy / ссылки — правки в **Unity Editor** ([unity-mcp.md](unity-mcp.md))
- Runtime domain (enum, flow state) — typed exception по [exceptions.md](exceptions.md)

## Именование переменных здесь

Смысл и единственность цели — в этом файле. Префиксы casing — только [codestyle.md](codestyle.md) (`camelCase` locals/params; `_camelCase` private fields). Без hungarian / type-prefix.

## Soft-check для агента

Перед новой переменной/полем: нужна ли она → минимальный scope → init рядом с use → одна цель → значение не «магия», а const/config по нужной гибкости. Ревью — [../../CLAUDE.md](../../CLAUDE.md) §4.
