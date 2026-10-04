# StockFlow — Project Brief & Progress Tracker

> ⚠️ **Правило оновлення файлу:** одразу після завершення кожного значущого
> кроку — онови ВСІ розділи файлу, де ця зміна відображена, а не лише
> "Поточний статус". Один факт (рішення, реалізована сутність, пройдений
> тест) часто зачіпає кілька розділів одночасно (Roadmap, Доменна модель,
> Нотатки) — Claude звіряє це за алгоритмом у розділі "🧭 Інструкція для
> Claude", а не за фіксованим переліком розділів. Не відкладай на "після
> завершення етапу цілком". Claude подає патчі у форматі:
> Розділ → Дія → Текст для копіювання.

---

## 🎯 Мета проєкту

Fullstack pet-проєкт для портфоліо рівня Middle, що демонструє:
- Володіння C#/.NET на бекенді з сучасною архітектурою
- Впевнені навички React на фронтенді
- Розуміння TDD (вибірково, за критерієм складності), DDD, CQRS, Clean Architecture
- Практики DevOps (Docker, CI/CD)

Розробник позиціонує себе як C#/.NET Developer, хоче закрити прогалину
у Frontend і показати архітектурну зрілість. Часовий горизонт: 3+ місяці.

**Назва проєкту: StockFlow**

---

## 🛠 Технологічний стек

**Backend:** ASP.NET Core Web API (.NET 9), Clean Architecture
(Domain/Application/Infrastructure/API), CQRS + MediatR, EF Core + PostgreSQL,
FluentValidation, ASP.NET Identity + JWT (access + refresh), Serilog, Hangfire,
- xUnit + NSubstitute (моки); стандартний `Assert.*` xUnit без сторонніх
  assertion-бібліотек (FluentAssertions спочатку планувався, але
  фактично не використовується з StockItem — план приведено у
  відповідність до коду)
- Testcontainers (інтеграційні тести з реальною PostgreSQL у Docker)
- Swashbuckle/Swagger (OpenAPI-документація + інтерактивний UI) —
  підключено явно через NuGet-пакет `Swashbuckle.AspNetCore`; вбудований
  у шаблон .NET 9 `Microsoft.AspNetCore.OpenApi` (`AddOpenApi()`/
  `MapOpenApi()`, лише JSON-специфікація без UI) видалено й замінено
  повністю
- WebApplicationFactory (functional/API-тести для критичних наскрізних сценаріїв)
- Postman (ручна колекція запитів для демонстрації API)

**Frontend:** React + TypeScript + Vite, TanStack Query, Zustand,
React Hook Form + Zod, MUI, Vitest + React Testing Library.

**DevOps:** Docker + docker-compose, GitHub Actions (CI/CD),
деплой Render/Railway (backend+DB) + Vercel/Netlify (frontend).

---

## 🏛 Архітектурні рішення

1. **Clean Architecture** — Domain нічого не залежить; Application залежить
   лише від Domain; Infrastructure/API залежать через інтерфейси (DIP).
   Domain-проєкт не має жодного зовнішнього NuGet-пакета.
2. **CQRS + MediatR** — кожен use case = окремий Command/Query + Handler.
3. **DDD (тактичні патерни)** — Entity, Value Object, Aggregate Root,
   Domain Events, Domain Service (за потреби) для ключових інваріантів.
4. **TDD — вибірково**, глибина залежить від ціни помилки в конкретній
   логіці, а не єдиний стандарт на весь домен (див. розділ TDD нижче).
5. **Repository + Unit of Work** — абстракція над EF Core; правило
   "один Repository — один Aggregate Root **або незалежно персистована
   проста сутність**". Друге уточнення додано свідомо: прості довідникові
   сутності (`Category`, `Warehouse`, `Supplier`) не є Aggregate Root
   (немає доменних подій, немає складних інваріантів), але кожна з них —
   окремий незалежно персистований набір записів (власний `DbSet`), а не
   дочірній об'єкт усередині чужого агрегату (як `PurchaseOrderLine`
   всередині `PurchaseOrder`) — тому кожна також отримує власний
   репозиторій, а не спільний generic-репозиторій "на все".
6. **Dependency Inversion** — інтерфейси репозиторіїв оголошуються в
   Application, реалізуються в Infrastructure; wiring відбувається в API
   (Program.cs, DI-контейнер).
7. **Value Objects** — реалізуються через `record` (не через базовий
   `abstract ValueObject` клас): компілятор дає рівність-за-значенням,
   `ToString()`, `Deconstruct` безкоштовно. Конструктор — приватний,
   створення лише через статичну фабрику `Create()` з валідацією.
   Властивості — **без `init`** (тільки `get`), positional-синтаксис
   record **не використовується** для полів з валідацією — це блокує
   обхід валідації через `with`-вираз (copy-constructor копіює поля
   напряму, минаючи публічний конструктор).
8. **Guard-логіка в Entity** — валідація живе у статичній фабриці
   `Create()`, приватний конструктор — лише присвоєння полів, без
   логіки (той самий патерн, що й для Value Objects у пункті 7,
   застосований і до Entity). Виняток: `PurchaseOrderLine`, де
   перевірка потрапила в конструктор — залишено як є заднім числом
   (гілка змержена й видалена), але це відхилення, не альтернативний
   стандарт.
9. **Application-шар — feature-based (vertical slice) структура.** Кожна
   сутність/агрегат отримує власну папку в корені `StockFlow.Application`
   (`Categories/`, `Products/`, `Warehouses/`, `Suppliers/`, `StockItems/`,
   `PurchaseOrders/`, `StockTransfers/`), а всередині — підпапки
   `Commands/<UseCase>/` і `Queries/<UseCase>/`, кожна з трьома файлами:
   `<UseCase>Command.cs`, `<UseCase>CommandHandler.cs`,
   `<UseCase>CommandValidator.cs`. Кросс-каттинг-код — у `Common/`
   (`Interfaces/`, `Behaviors/`, `Exceptions/`), не в папках сутностей.
10. **Валідація команд — через MediatR pipeline behavior**
    (`ValidationBehavior<TRequest,TResponse>` у `Common/Behaviors/`), не
    ручний виклик `validator.Validate()` в кожному Handler'і. За наявності
    помилок кидається власний `StockFlow.Application.Common.Exceptions.
    ValidationException` (обгортка над `FluentValidation.Results.
    ValidationFailure`, не сам виняток FluentValidation) — його пізніше
    перехопить `ExceptionHandlerMiddleware` (буде додано разом із першим
    контролером) і перетворить на `400 Bad Request`.
11. **Ієрархія доменних винятків — `DomainException` (abstract) лише для
    "іменованих" бізнес-правил, не для простих guard-клауз (Варіант Д).**
    `DomainException` — спільний базовий клас для 9 раніше створених
    іменованих класів (`InsufficientStockException`,
    `InsufficientAvailableStockException`, `OverReceiptException`,
    `EmptyPurchaseOrderException`, `EmptyStockTransferException`,
    `InvalidPurchaseOrderStatusTransitionException`,
    `InvalidStockTransferStatusTransitionException`,
    `PurchaseOrderLineNotFoundException`, `SameWarehouseTransferException`)
    — кожен зберігає власний контекстний конструктор
    (orderId/currentStatus/quantityOrdered тощо), лише базовий клас
    змінюється з `Exception` на `DomainException`.

    **Розведення "invalid argument" vs "business rule" — збережено, не
    скасовано.** Уже задокументований раніше принцип (розділ "Нотатки
    та рішення": StockItem, PurchaseOrderLine, PurchaseOrder.AddLine)
    лишається чинним: `ArgumentException`/`ArgumentOutOfRangeException` —
    для некоректного вхідного параметра виклику (Guid.Empty, дублікат
    ProductId, значення поза діапазоном), НЕ доменний виняток. Жодна
    проста guard-клауза не переводиться на `DomainException`-нащадків.

    `ExceptionHandlingMiddleware` ловить **обидва сімейства** винятків
    окремими `catch`-блоками — `DomainException` ("Business rule
    violation") і `ArgumentException`/`ArgumentOutOfRangeException`
    ("Invalid argument") — обидва повертають `400 Bad Request`, з
    різним `title` у тілі відповіді для діагностичної прозорості.

    Клас `DomainValidationException`, запропонований на попередньому
    кроці обговорення (Варіант Б1/В1), **скасовано й не використовується**
    — рішення переглянуто одразу після звірки з файлом плану, щойно
    виявлено вже задокументоване розведення, яке В1 порушував би.

---

## 📁 Структура репозиторію

Джерело правди для розташування файлів — звіряти з цим деревом, а не
вигадувати шлях заново в кожній сесії:
StockFlow/
├── .gitignore
├── README.md
├── docker-compose.yml
├── StockFlow.sln
├── src/
│ ├── StockFlow.Domain/
│ │ ├── Entities/
│ │ ├── ValueObjects/
│ │ ├── Enums/
│ │ ├── Events/
│ │ ├── Exceptions/
│ │ └── StockFlow.Domain.csproj
│ ├── StockFlow.Application/
│ │ ├── DependencyInjection.cs
│ │ ├── Common/
│ │ │ ├── Interfaces/ ← IRepository/IUnitOfWork тощо (Етап 4)
│ │ │ ├── Behaviors/ ← ValidationBehavior
│ │ │ └── Exceptions/ ← ValidationException (Application-level)
│ │ ├── Products/
│ │ ├── Categories/
│ │ ├── Warehouses/
│ │ ├── Suppliers/
│ │ ├── StockItems/
│ │ ├── PurchaseOrders/
│ │ ├── StockTransfers/
│ │ └── StockFlow.Application.csproj
│ ├── StockFlow.Infrastructure/
│ │ ├── Persistence/
│ │ │ ├── Configurations/
│ │ │ ├── Migrations/
│ │ │ └── Repositories/
│ │ ├── Identity/
│ │ ├── BackgroundJobs/
│ │ ├── DependencyInjection.cs ← за тим самим патерном, з'явиться Етап 4
│ │ └── StockFlow.Infrastructure.csproj
│ └── StockFlow.Api/
│ ├── Controllers/
│ ├── Middleware/
│ ├── Program.cs
│ ├── appsettings.json
│ └── StockFlow.Api.csproj
├── tests/
│ ├── StockFlow.Domain.UnitTests/
│ │ └── Entities/<Сутність>Tests.cs
│ ├── StockFlow.Application.UnitTests/
│ │ └── <Feature>/Commands/<UseCase>/<UseCase>CommandValidatorTests.cs
│ ├── StockFlow.Infrastructure.IntegrationTests/
│ └── StockFlow.Api.FunctionalTests/ ← з'явиться разом із першим контролером
└── client/ ← ініціалізуємо на Етапі 6 (React)

`DependencyInjection.cs` — один файл у корені кожного проєкту (не в
підпапці), бо це точка входу конфігурації шару, а не бізнес-логіка.

## 🧱 Спільні базові класи (Domain/Common)

Точний код нижче — джерело правди для сигнатур, які мають успадковувати
всі Entity/AggregateRoot проєкту. Будь-яка нова сутність звіряється з
цими сигнатурами, а не вигадується заново в кожному чаті.

**`AggregateRoot<TId>`** (`src/StockFlow.Domain/Common/AggregateRoot.cs`):
```csharp
namespace StockFlow.Domain.Common
{
    public abstract class AggregateRoot<TId> : Entity<TId>
    {
        private readonly List<IDomainEvent> _domainEvents = new();
        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        protected AggregateRoot() { }
        protected AggregateRoot(TId id) : base(id) { }

        protected void RiseDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);

        public void ClearDomainEvents() => _domainEvents.Clear();
    }
}
```
Наслідки для будь-якого нового Aggregate Root:
- Успадковується як `AggregateRoot<Guid>` (конкретизація `TId` = `Guid`
  для всього проєкту — жодна сутність не використовує інший тип Id).
- `Id` передається через `base(id)` у приватному конструкторі —
  **не** оголошується власним полем `Id` у нащадку.
- Для доменних подій — захищений метод `RiseDomainEvent(...)`
  (саме таке написання в коді, з друкарською помилкою "Rise" замість
  "Raise" — фіксується як є, не виправляється заднім числом без
  окремого рефакторинг-рішення).

**`Entity<TId>`** та **`IDomainEvent`** — використовуються
(`AggregateRoot<TId>` успадковує перший, посилається на другий), але
їхній точний код ще не задокументований тут. Додати за першої ж нагоди,
коли знадобиться сутність, що успадковує `Entity<TId>` напряму (не
через `AggregateRoot`), або коли створюватиметься перша конкретна
доменна подія.

**Ієрархія винятків (фінальна, Варіант Д):**
Exception
├── ArgumentException / ArgumentOutOfRangeException ← прості guard-клаузи (без змін, як і раніше)
└── DomainException (abstract, src/StockFlow.Domain/Exceptions/DomainException.cs)
├── InsufficientStockException
├── InsufficientAvailableStockException
├── OverReceiptException
├── EmptyPurchaseOrderException
├── EmptyStockTransferException
├── InvalidPurchaseOrderStatusTransitionException
├── InvalidStockTransferStatusTransitionException
├── PurchaseOrderLineNotFoundException
└── SameWarehouseTransferException

Лише 9 іменованих класів переходять на `DomainException`; `ArgumentException`/
`ArgumentOutOfRangeException` — паралельна, окрема гілка, не об'єднана
з `DomainException`.

## 🧱 Спільні базові класи (Application/Common)

Точний код нижче — джерело правди, аналогічно Domain/Common.

**`ValidationBehavior<TRequest,TResponse>`**
(`src/StockFlow.Application/Common/Behaviors/ValidationBehavior.cs`):
перехоплює кожен Command/Query через MediatR pipeline, запускає всі
зареєстровані `IValidator<TRequest>`, за наявності помилок кидає
`ValidationException`. Реєструється в `DependencyInjection.cs` через
`cfg.AddOpenBehavior(typeof(ValidationBehavior<,>))`.

**`ValidationException`**
(`src/StockFlow.Application/Common/Exceptions/ValidationException.cs`):
власний клас (не з FluentValidation), `Errors` — `IDictionary<string,
string[]>`, згруповані по `PropertyName`. Це те, що зловить майбутній
`ExceptionHandlerMiddleware`.

> Походження: типовий шаблон з референсних Clean Architecture рішень
> для .NET (MediatR + FluentValidation), не власна розробка проєкту —
> форма `Errors` навмисно повторює `ValidationProblemDetails` з ASP.NET
> Core, щоб middleware міг віддати її як є у відповідь `400`. Джерело:
> [jasontaylordev/CleanArchitecture](https://github.com/jasontaylordev/CleanArchitecture)
> (Clean Architecture Solution Template, де-факто еталон для цієї
> архітектури в .NET-спільноті).

## 🧩 Інтерфейси репозиторіїв та Unit of Work (Application/Common/Interfaces)

Джерело правди для сигнатур — звіряти тут, а не вигадувати заново.
Кожен інтерфейс оголошується в Application, реалізується в Infrastructure
(Етап 4), wiring — у `Infrastructure.DependencyInjection`.

**Дизайн-рішення (обговорено явно):** обрано "тонкий" `IUnitOfWork`
(лише `SaveChangesAsync`), репозиторії й `IUnitOfWork` інжектяться в
Handler окремо, незалежно один від одного — НЕ "товстий" UoW з
властивостями-репозиторіями (`unitOfWork.Categories` тощо). Причини:
(1) Open/Closed — новий репозиторій не вимагає правити сам
`IUnitOfWork`; (2) послідовність із тим, як репозиторій уже
інжектується окремо у `CreateCategoryCommandValidator`; (3) YAGNI/Rule
of Three, за аналогією з `EnsurePositiveQuantity`/`EnsureNotEmpty`.

> Примітка щодо джерела (правило 5a): сам репозиторій+UoW підхід —
> стандартний DDD-патерн (Evans, Fowler — "Patterns of Enterprise
> Application Architecture"), не власна розробка проєкту, але
> **відрізняється** від шаблону Джейсона Тейлора, який взагалі не
> використовує ні Repository, ні Unit of Work — там Handler напряму
> інжектить `IApplicationDbContext` і викликає `SaveChangesAsync()`.
> Це наше свідоме відхилення від того референсу, узгоджене ще до появи
> цього референсу в чаті (п. 5 "Архітектурних рішень").

| Інтерфейс | Файл | Статус | Коментар |
|---|---|---|---|
| `ICategoryRepository` | `Common/Interfaces/ICategoryRepository.cs` | 🔶 | `ExistsAsync(Guid id)` + `AddAsync(Category category)`; `Id` генерується в домені (`Category.Create()`), тому `AddAsync` нічого не повертає; `GetByIdAsync` свідомо не додано (YAGNI — немає ще use case, що його потребує) |
| `IUnitOfWork` | `Common/Interfaces/IUnitOfWork.cs` | 🔶 | Лише `SaveChangesAsync(CancellationToken)`; реалізація — Етап 4 (обгортка над `DbContext`) |

## 📦 Доменна модель

**Легенда статусу реалізації** (використовується в таблицях нижче):
⬜ Не розпочато · 🔶 Дизайн погоджено, код не написано · 🟡 TDD в процесі · ✅ Реалізовано й повністю протестовано

### Сутності

| Сутність | Ключові поля | Статус | Коментар |
|---|---|---|---|
| Product | Id, SKU (SkuValue), Name, Description, CategoryId, UnitOfMeasure (enum), Barcode | ✅ | Create() з guard-ами (name, categoryId); Barcode опціональний; SKU-унікальність — Application-шар |
| Category | Id, Name, ParentCategoryId | ✅ | Self-reference ієрархія; Name — Trim() при створенні; перевірка існування батька й циклів — Application-шар, не Domain |
| Warehouse | Id, Name, Address, IsActive | ✅ | Create() з guard-ами (name, address) + Trim()-нормалізація обох полів; IsActive = true за замовчуванням, керується окремими методами (Activate/Deactivate) пізніше |
| Supplier | Id, Name, ContactEmail (EmailValue), Phone | ✅ | Create() з guard-ами (name, phone) + Trim()-нормалізація обох полів; ContactEmail валідний за визначенням типу (EmailValue) |
| StockItem (Aggregate Root) | ProductId, WarehouseId, QuantityOnHand, QuantityReserved, MinimumStockLevel | ✅ | Create/Receive/Deduct/Reserve/ReleaseReservation — повністю реалізовано й протестовано |
| StockMovement | Id, ProductId, WarehouseId, Type (MovementType enum), Quantity, Reason, CreatedAt, CreatedByUserId | ✅ | Immutable audit-запис; Create() з EnsureNotEmpty-хелпером (Guid-поля) + guard на Quantity > 0; CreatedAt виставляється доменом (UtcNow), не приймається ззовні |
| PurchaseOrder | SupplierId, WarehouseId, Status, OrderDate, ExpectedDeliveryDate, Lines | ✅ | AggregateRoot; стан-машина Draft→Sent→PartiallyReceived/Received, Cancel лише з Draft/Sent; AddLine/Send/ReceiveLine/Cancel; 16 тестів |
| PurchaseOrderLine | ProductId, QuantityOrdered, QuantityReceived, UnitPrice | ✅ | Дочірній обʼєкт (не aggregate root, без власного репозиторію); Create() з guard-ами (ProductId≠Empty, quantityOrdered>0, unitPrice≥0); QuantityReceived стартує з 0, змінюється лише через майбутній метод отримання; 10 тестів |
| StockTransfer (Aggregate Root) | FromWarehouseId, ToWarehouseId, Status, Lines[], CreatedAt | ✅ | Create/AddLine/Ship/Complete/Cancel — повністю реалізовано й протестовано; 13 тестів |
| StockTransferLine | ProductId, Quantity | ✅ | Дочірній обʼєкт (не aggregate root); Create() з guard-ами (ProductId≠Empty, Quantity>0); 3 тести |
| ApplicationUser | через ASP.NET Identity | ⬜ | Ролі: Admin, Manager, WarehouseWorker |

### Value Objects

| Value Object | Формат/значення | Статус | Коментар |
|---|---|---|---|
| SkuValue | `record`, приватний ctor + `Create()`; regex `^[A-Z0-9-]+$`, довжина 3–30 | ✅ | Готово: 5 тестів (valid, null/empty, length, format, equality) |
| UnitOfMeasure | enum: `Pcs`, `Kg`, `L`, `M` | ✅ | Реалізовано, файл `Enums/UnitOfMeasure.cs` |
| MovementType | enum: `Receipt`, `Issue`, `Transfer`, `Adjustment` | ✅ | Реалізовано, файл `Enums/MovementType.cs` |
| EmailValue | Формат email (спрощений regex, без повної RFC 5322-відповідності) | ✅ | record, приватний конструктор, Create() з форматною валідацією; equality — по Value |
| StockTransferStatus | enum: `Draft`, `InTransit`, `Completed`, `Cancelled` | 🔶 | Реалізовано, файл `Enums/StockTransferStatus.cs`; `Cancelled` додано під час TDD на Cancel() — не було в первинному дизайні стан-машини (Draft→InTransit→Completed), додано без порушення послідовності переходів "вперед" |

### Aggregate Roots (межі узгодженості)

- **StockItem** — власний агрегат; StockMovement — дочірній immutable-запис.
- **Product** — власний агрегат; Category — проста сутність/довідник.
- **PurchaseOrder** — агрегат, PurchaseOrderLine — дочірній обʼєкт.
- **StockTransfer** — окремий агрегат.
- **Warehouse**, **Supplier** — прості довідникові агрегати.

Жоден Handler не змінює два агрегати "одним махом" через прямі посилання —
кожен оновлюється окремим викликом, узгодженість забезпечує Unit of Work.

- **Category, Warehouse, Supplier** — не Aggregate Root (прості
  довідникові сутності), але кожна отримує власний Repository в
  Application/Infrastructure (див. "Архітектурні рішення", п. 5) —
  перше рішення прийнято для `Category` (`ICategoryRepository`), щоб
  `CreateCategoryCommandValidator` міг перевірити існування
  `ParentCategoryId`.

### Бізнес-правила (інваріанти)

- `QuantityOnHand` ніколи не від'ємний.
- `QuantityReserved` не може перевищувати `QuantityOnHand`.
- Залишок змінюється лише через створення `StockMovement`.
- SKU унікальний у межах системи — перевірка **в Application-шарі**
  (Domain-сутність не має доступу до інших записів для такої перевірки).
- Отримання по `PurchaseOrder` не може перевищити замовлену кількість.
- `StockTransfer` — дві проводки в одній транзакції; `FromWarehouseId != ToWarehouseId`;
  стан-машина `Draft → InTransit → Completed`; `Cancel()` дозволено лише з
  `Draft`/`InTransit` (не з `Completed` — залишки вже реально оновлені на
  обох складах, скасування звідти суперечило б фактичному стану інвентарю).
- `StockTransfer.AddLine()` не дозволяє дублікат `ProductId` в межах
  одного переміщення (за аналогією з `PurchaseOrder.AddLine()`) —
  `ArgumentException`, некоректний вхідний параметр виклику, не
  порушення стану агрегату. Партії/лоти того самого товару на MVP не
  підтримуються (немає `BatchNumber`/`LotId` в `StockTransferLine`).
- `QuantityOnHand < MinimumStockLevel` → `LowStockDetectedEvent`.
- `PurchaseOrder` створюють лише Admin/Manager; `WarehouseWorker` лише реєструє рух.
- `PurchaseOrder.Cancel()` дозволено лише зі статусів `Draft`/`Sent`;
  скасування з `PartiallyReceived` заборонено (частина товару вже фізично
  надійшла на склад) — MVP не підтримує часткове скасування, це
  stretch-фіча `CancelRemaining()`.
- `PurchaseOrder.Send()` вимагає непорожній `Lines[]` — не можна відправити
  постачальнику замовлення без жодного рядка.

### Use cases (MVP)

1. CRUD товарів і категорій
2. CRUD складів
3. CRUD постачальників
4. Ручна реєстрація руху товару
5. Purchase Order: створення → отримання → оновлення залишку
6. Переміщення товару між складами
7. Дашборд поточних залишків
8. Сповіщення про низькі залишки
9. Audit log руху товару з фільтрами
10. Автентифікація + рольовий доступ

**Stretch:** звітність (оборотність, вартість запасів), SignalR live-оновлення, ABC-аналіз.

### Доменні події

`StockReceivedEvent`, `StockIssuedEvent`, `StockTransferredEvent`,
`LowStockDetectedEvent`, `PurchaseOrderCreatedEvent`, `PurchaseOrderReceivedEvent`

---

## 🧪 Підхід до тестування

**Критерій глибини TDD:** визначається ціною помилки в конкретній логіці,
а не єдиним стандартом для всього домену.

- **Висока ціна помилки** (арифметика, гроші, облік залишків) → повне
  покриття, включно з `[Theory]`-тестами на кожен guard-кейс.
  Приклад: StockItem.
- **Низька ціна помилки** (прості перевірки "не порожнє", без арифметики
  чи перехресних залежностей) → легший набір базових `[Fact]`-тестів.
  Приклад: Product.
- Виняток можливий навіть у "простій" сутності, якщо є специфічна логіка
  (наприклад нормалізація) — тоді саме ця частина отримує трохи більше уваги.

**Пиши тест першим (TDD, Red-Green-Refactor):**
- Поведінка Aggregate Root з реальними інваріантами.
- Command Handlers з розгалуженнями/побічними ефектами.
- Валідатори для складних правил.
- Frontend: Zod-схеми, кастомні хуки з нетривіальною логікою.

**НЕ варто робити TDD:**
- Прості CRUD без бізнес-правил.
- EF Core конфігурації, репозиторії — інтеграційні тести (Testcontainers)
  після стабілізації контракту.
- "Вітринні" React-компоненти без логіки в рендері.

**Тести — DAMP, не DRY:** для однакових guard-перевірок у різних методах
одного агрегату пишемо окремі `[Theory]`-тести замість одного узагальненого
через делегати — назва тесту одразу вказує, який метод впав, без пошуку в
дебагері. DRY застосовується в продакшн-коді (напр. спільний guard-метод),
але не обов'язково в тестах.

**Коли виносити guard у приватний helper-метод (DRY у production-коді):**
лише після того, як та сама перевірка реально задублювалась у 2+ місцях
у межах одного класу (Rule of Three / YAGNI) — не заздалегідь "про
запас" для єдиного виклику. Так з'явились `EnsurePositiveQuantity`
(StockItem) і `EnsureNotEmpty` (StockMovement) — обидва після появи
дублювання, а не до нього.


**Чотири рівні піраміди тестування в проєкті:**

1. **Unit** (Domain, Application) — TDD, глибина за критерієм ціни помилки (див. вище).
2. **Integration** (Infrastructure) — Testcontainers, після стабілізації контракту.
3. **Functional/API** (WebApplicationFactory) — лише для критичних наскрізних
   сценаріїв, де важливий саме повний ланцюжок HTTP→Auth→Handler→БД
   (напр. PurchaseOrder receive оновлює StockItem, 401 на захищеному
   ендпоінті без токена, атомарність StockTransfer). НЕ для простих CRUD
   без бізнес-правил — там немає додаткової цінності понад Unit/Integration.
4. **Manual** (Swagger UI, Postman) — ручна перевірка під час розробки та
   демонстрація проєкту; Postman-колекція експортується наприкінці, коли
   API стабілізується.

---

## 🔀 Git-коміти

**Коли комітити:** на межах TDD-циклу, не довільно.
- Після Red — НЕ комітити (непрацюючий стан).
- Після Green — коміт-кандидат (тест написано, код працює).
- Після Refactor — окремий коміт, якщо зміна суттєва; тривіальний
  рефакторинг можна обʼєднати з Green-комітом.

**Правило:** один коміт = один логічно завершений крок. Якщо повідомлення
хочеться написати через "і" (додав тест І реалізував фічу І щось виправив) —
розбий на кілька комітів.

**Формат — Conventional Commits:**
- `feat:` — нова функціональність
- `test:` — тести без зміни production-коду
- `refactor:` — зміна структури без зміни поведінки
- `fix:` — виправлення бага
- `chore:` — технічне (налаштування, залежності)
- `docs:` — документація (включно з оновленням цього MD-файлу)

**Критерій вибору між `feat:` і `test:` (уточнено після виявленої
непослідовності):** визначає ЛИШЕ те, чи змінився production-код у
конкретному коміті — а не факт, що це TDD-крок.
- Green-коміт, де тест супроводжується новим/зміненим production-кодом
  (новий клас, guard, метод) → **`feat:`**, навіть якщо в тому самому
  коміті йде й тест-файл — це нормально для TDD.
- Коміт додає/змінює ЛИШЕ тест-файл, жодного production-файлу (напр.
  додатковий щасливий шлях чи регресійний тест на вже написаний код,
  тест виявився одразу Green) → **`test:`**.

**Коли TDD не застосовується** (базові класи, EF-конфігурації, DI-wiring,
прості CRUD без бізнес-правил): коміт робиться, коли одиниця роботи
завершена, компілюється і логічно цілісна сама по собі — без циклу
Red-Green. Приклад: `feat: add Entity, AggregateRoot, IDomainEvent base
classes` — один коміт, без попереднього `test:`.

**Уточнення гранулярності для TDD-сутностей:** один тест = один
Red-Green(-Refactor) цикл = один окремий коміт. Не групувати кілька
тестів в один коміт "на всю сутність" — навіть якщо всі вони зелені
на момент коміту. Виняток: тривіальний Refactor після Green можна
об'єднати з тим самим комітом (як і зазначено вище), але два різні
тести — це завжди мінімум два різні коміти.

**Спільна вимога для обох випадків:** ніколи не комітити код, що не
компілюється або ламає вже існуючі тести — це єдине жорстке правило,
незалежне від того, TDD це чи ні.


**Гілки (branching workflow):** кожна нова сутність домену або значний
крок Roadmap розробляється в окремій feature-гілці, а не напряму в `main`.

- Назва гілки: `feature/<коротка-назва>`, напр. `feature/product-domain`,
  `feature/purchase-order-domain`.
- Створення: `git checkout -b feature/<назва>` від актуального `main`.
- Всередині гілки коміти робляться так само часто й атомарно, як описано
  вище (TDD-цикли, логічні кроки) — гілка не звільняє від дисципліни
  дрібних комітів, навпаки, дає для цього безпечний простір.
- Завершення роботи над сутністю: `git push -u origin feature/<назва>`,
  потім Pull Request на GitHub у `main`, самостійний рев'ю опису PR,
  merge **звичайним merge-комітом (не squash)** — так історія окремих
  TDD-циклів залишається видимою в `main`, а не стискається в один коміт.
  Це свідомий вибір саме для портфоліо: рев'юєр повинен бачити процес.
  
**Порядок дій при завершенні сутності (жорстка послідовність, не міняти
місцями):**
1. Останній Green + коміт коду.
2. Патчі до `docs/StockFlow_Plan.md` → застосувати → окремий коміт
   `docs: update plan for <сутність> completion` **на тій самій
   feature-гілці**, до push.
3. `git push -u origin feature/<назва>`.
4. Pull Request → merge (не squash) → видалення гілки.

Docs-коміт про завершення сутності — це частина PR цієї сутності, а не
дія "після". Якщо гілку вже змержено й видалено до того, як план
оновлено (як сталося з PurchaseOrder) — виправляти доводиться окремим
ретроактивним docs-комітом напряму в `main`, поза межами PR — цього
варто уникати наперед.

**Claude підказує момент і текст коміту сама**, користувач ще напрацьовує
відчуття, коли саме комітити — не покладатись на те, що він сам це визначить.

---

## 🗺 Повний план (Roadmap)

### Етап 0 + Етап 2 — Підготовка та архітектура рішення (ОБʼЄДНАНО)
- [x] Репозиторій, git ініціалізовано
- [x] Солюшн + 4 backend-проєкти (Domain/Application/Infrastructure/Api)
- [x] Посилання між шарами налаштовано (Application→Domain, Infrastructure→Application, Api→обидва)
- [x] Тестові проєкти (xUnit) для Domain/Application/Infrastructure
- [x] Внутрішня структура папок створена
- [x] .gitignore, README, перший коміт
- [x] Доменна модель затверджена
- [x] Теорія: Clean/Onion Architecture, Dependency Inversion — розібрано

### Етап 1 — Моделювання домену
- [x] Common: `Entity<TId>`, `AggregateRoot<TId>`, `IDomainEvent`
- [x] StockItem повністю: Create/Receive/Deduct/Reserve/ReleaseReservation
      + guard-валідація (`EnsurePositiveQuantity`) + exceptions
      (`InsufficientStockException`, `InsufficientAvailableStockException`)
      + повне TDD-покриття (включно з `[Theory]`)
- [x] Доменну модель деталізовано: описи полів, aggregate roots, ERD-діаграма
- [x] Теорія DDD поглиблено: Bounded Context, Ubiquitous Language,
      Aggregate (транзакційна межа), Domain Service, Domain Event
- [x] Product — SkuValue (5 тестів), UnitOfMeasure (enum), Product entity
      (3 тести: valid, empty name, empty categoryId)
- [x] Category повністю: Create() з guard (name) + Trim()-нормалізація
      + self-reference через nullable ParentCategoryId (4 тести)
- [x] Warehouse повністю: Create() з guard-ами (name, address) +
      Trim()-нормалізація + IsActive = true за замовчуванням (5 тестів)
- [x] EmailValue (VO): формат email, guard на null/empty, equality (8 тестів)
- [x] Supplier повністю: Create() з guard-ами (name, phone) +
      Trim()-нормалізація обох полів, ContactEmail типізований через
      EmailValue
- [x] MovementType (enum): Receipt, Issue, Transfer, Adjustment
- [x] StockMovement повністю: immutable entity, Create() з
      EnsureNotEmpty-хелпером + guard на Quantity > 0, CreatedAt
      виставляється доменом (7 тестів, включно з Refactor-кроком)
- [x] PurchaseOrder + PurchaseOrderLine повністю: Create/AddLine/Send/
      ReceiveLine/Cancel, стан-машина Draft→Sent→PartiallyReceived/
      Received, Cancel лише з Draft/Sent (26 тестів разом:
      PurchaseOrderLine 10 + PurchaseOrder 16)
- [x] StockTransfer + StockTransferLine повністю: Create/AddLine/Ship/
      Complete/Cancel, стан-машина Draft→InTransit→Completed, Cancel
      лише з Draft/InTransit (16 тестів разом: StockTransferLine 3 +
      StockTransfer 13)

### Етап 3 — Backend: ядро логіки (CQRS + MediatR)
- [x] Теорія: CQRS, Mediator, Command vs Query
- [x] Структура папок Application-шару узгоджена (feature-based:
      Categories/, Products/ тощо + Common/{Interfaces,Behaviors,Exceptions})
      — див. розділ "📁 Структура репозиторію"
- [x] MediatR + FluentValidation: NuGet-пакети, DI-wiring (`AddApplication()`),
      `ValidationBehavior` + `Application.Common.Exceptions.ValidationException`
- [ ] Commands/Queries для use cases 1–9 (**use case 1, CRUD категорій:**
      `CreateCategoryCommand` ✅ повністю — Command + Validator (4 тести)
      + Handler (1 тест); `GetCategoryById`/`Update`/`Delete` ще не
      розпочато)
- [x] Перший контролер (`CategoriesController`, `POST /api/Categories`) +
      `ExceptionHandlingMiddleware` написано й закомічено. Swagger UI
      підключено (Swashbuckle), але **реальний запуск застосунку наразі
      неможливий** — `ICategoryRepository`/`IUnitOfWork` не мають
      реалізації (заплановано на Етап 4), DI-контейнер коректно падає
      при старті з `Unable to resolve service`. Це очікувана, а не
      помилкова поведінка (fail-fast валідація DI) — Варіант А
      (почекати Етап 4) свідомо обрано замість тимчасової in-memory
      заглушки. Наскрізна перевірка через Swagger UI відкладена до
      появи реальної Infrastructure-реалізації
- [ ] Проєкт `tests/StockFlow.Api.FunctionalTests` (WebApplicationFactory) —
      перші тести для критичних сценаріїв, зростає разом з API

### Етап 4 — Персистентність (EF Core + PostgreSQL)
- [ ] DbContext, конфігурації, міграції, Repository/UoW
- [ ] Інтеграційні тести з Testcontainers
- [ ] Теорія: Repository pattern, N+1 проблема

### Етап 5 — Автентифікація й авторизація
- [ ] JWT + Refresh tokens, ролі, policy-based авторизація
- [ ] Swagger Security Definition для JWT (Bearer-токен прямо в Swagger UI)
- [ ] Functional-тест: 401 на захищеному ендпоінті без токена
- [ ] Теорія: JWT, RBAC vs claims-based

### Етап 6 — Frontend: базова структура
- [ ] Vite, роутинг, feature-based структура
- [ ] TanStack Query
- [ ] Теорія: feature-based структура, кешування/інвалідація

### Етап 7 — Frontend: інтерфейс і форми
- [ ] Списки, форми (RHF + Zod), дашборд залишків
- [ ] Тести компонентів (Vitest + RTL) там, де є логіка
- [ ] Теорія: контрольовані форми, оптимістичні оновлення UI

### Етап 8 — Тестування, DevOps, деплой
- [ ] Postman-колекція (експорт, додати в репозиторій)
- [ ] Docker + docker-compose
- [ ] GitHub Actions (CI/CD) — прогін усіх 3 рівнів автоматизованих тестів
- [ ] Деплой у хмару
- [ ] Теорія: піраміда тестування, основи CI/CD

### Stretch (після MVP)
- [ ] SignalR live-оновлення
- [ ] Базова звітність / ABC-аналіз

---

## 📍 Поточний статус

- [x] Тема: Warehouse / Inventory Management System
- [x] Назва: **StockFlow**
- [x] Стек: .NET 9 + React/TypeScript
- [x] Часовий горизонт: 3+ місяці
- [x] TDD — вибірково, критерій "ціна помилки"
- [x] Доменна модель підтверджена
- [x] Етап 0 + Етап 2 — виконано повністю
- [x] Етап 1 — завершено повністю (усі сутності домену реалізовані):
  - [x] Common-класи + StockItem (повністю, з тестами)
  - [x] Product: SkuValue, UnitOfMeasure, Product entity — змерджено в
        main через PR (feature/product видалено, локально й на remote)
  - [x] Category: Create() з guard (name) + Trim()-нормалізація
        + self-reference через nullable ParentCategoryId (4 тести) —
        змерджено в main через PR (feature/category-domain видалено,
        локально й на remote)
  - [x] Warehouse: Create() з guard-ами (name, address) +
        Trim()-нормалізація обох полів + IsActive = true за
        замовчуванням (5 тестів) — змерджено в main через PR
        (feature/warehouse-domain видалено, локально й на remote)
  - [x] Supplier: EmailValue (VO, 8 тестів) + Supplier entity з
        guard-ами (name, phone) і Trim()-нормалізацією обох полів —
        змерджено в main через PR (feature/supplier-domain видалено,
        локально й на remote)
  - [x] StockMovement: MovementType (enum) + immutable entity з
        EnsureNotEmpty-хелпером і guard на Quantity > 0 (7 тестів) —
        змерджено в main через PR (feature/stock-movement-domain
        видалено, локально й на remote)
  - [x] PurchaseOrder + PurchaseOrderLine: повна стан-машина
        (Create/AddLine/Send/ReceiveLine/Cancel), 26 тестів разом —
        змерджено в main через PR (feature/purchase-order-domain
        видалено, локально й на remote); docs-оновлення плану
        закомічено окремо (ретроактивно, поза PR — див. "Нотатки")
  - [x] StockTransfer + StockTransferLine: повна стан-машина
        (Create/AddLine/Ship/Complete/Cancel), 16 тестів разом —
        готово до push/PR (feature/stock-transfer-domain)
- [x] Етап 3 (у процесі): MediatR + FluentValidation підключено й
      налаштовано (DI-wiring, ValidationBehavior, Application-level
      ValidationException); структура папок Application узгоджена
- [x] `CreateCategoryCommand` повністю реалізовано: Command, Validator
      (4 тести: порожнє імʼя, неіснуючий батько, існуючий батько, без
      батька), Handler (1 тест) — готово до push/PR
      (`feature/create-category-command`)
- [x] Контролер + Swashbuckle + ExceptionHandlingMiddleware написано;
      запуск застосунку блокується відсутністю Infrastructure-реалізації
      `ICategoryRepository`/`IUnitOfWork` (очікувано, Варіант А)
- [x] Варіант Д остаточно узгоджено: `DomainException` лише для 9
      іменованих бізнес-винятків; `ArgumentException`/
      `ArgumentOutOfRangeException` — без змін, розведення збережено
- [x] Варіант Д реалізовано повністю: `DomainException` створено, 9
      іменованих винятків переведено на новий базовий клас,
      `ExceptionHandlingMiddleware` розширено на три catch-блоки
      (`ValidationException` → 400 "Validation failed",
      `DomainException` → 400 "Business rule violation",
      `ArgumentException`/`ArgumentOutOfRangeException` → 400
      "Invalid argument")
- [ ] **ПОТОЧНИЙ КРОК: завершити гілку `feature/domain-exception-hierarchy`
      (push/PR/merge); далі — повернутись у межі Етапу 3, use case 1
      (Get/Update/Delete категорій) або наступний use case (Products)**
- [ ] Етапи 4–8 не розпочато

---

## 📝 Нотатки та рішення

- StockItem.Create() — приватний конструктор, публічна лише фабрика Create().
- Розділення виключень: InsufficientStockException / InsufficientAvailableStockException
  — бізнес-правило (мапиться на конкретну HTTP-відповідь в Application-шарі);
  ArgumentOutOfRangeException — невалідний вхідний параметр, не бізнес-сценарій.
- Guard-логіка (quantity <= 0) винесена в приватний EnsurePositiveQuantity —
  DRY у продакшн-коді, але НЕ в тестах (там DAMP — 4 окремих Theory-тести).
- SKU-унікальність перевіряється в Application-шарі, не в Domain-сутності.
- Value Objects — через `record` з приватним конструктором і фабрикою
  `Create()`, без `init`-властивостей (позиційний синтаксис record не
  використовується там, де є валідація) — це навмисно блокує обхід
  валідації через `with`-вираз.
- SkuValue: формат — великі латинські літери, цифри, дефіс
  (`^[A-Z0-9-]+$`), довжина 3–30 символів, без пробілів на межах.
- UnitOfMeasure — enum, а не VO чи вільний string, бо це закритий
  контрольований набір значень.
- Category — проста сутність (Entity<Guid>), не AggregateRoot: без власних
  доменних подій. ParentCategoryId — nullable Guid без валідації існування
  чи циклів у Domain-шарі — це відповідальність Application-шару (за
  аналогією з SKU-унікальністю), бо Domain не має доступу до інших записів.
  Name нормалізується через Trim() у Create() — приклад "винятку" з
  розділу TDD: проста сутність, але специфічна логіка (нормалізація)
  отримала окремий TDD-раунд.
- Warehouse — проста довідникова сутність (Entity<Guid>). IsActive не
  приймається як параметр Create() — домен сам встановлює true за
  замовчуванням; зміна стану (деактивація/активація) винесена в окремі
  методи, які зʼявляться під час реалізації use case "CRUD складів".
  Name та Address нормалізуються через Trim() — той самий підхід, що й
  для Category.Name.
- ContactEmail у Supplier реалізований як окремий Value Object
  (EmailValue), а не простий string — за зразком SkuValue: чіткий,
  стандартний формат виправдовує власний VO з форматною валідацією.
  Phone, натомість, лишився простим string з guard "не порожнє" +
  Trim() — суворий формат телефону надто варіативний (коди країн тощо)
  для MVP.
- Barcode (Product) — опціональне поле (`string?`), без валідації формату
  на MVP. Причина: не всі товари фізично мають штрихкод (ваговий товар,
  кастомні позиції). Кандидат на окремий VO в майбутньому, якщо
  з'явиться потреба (напр. інтеграція зі сканером).
- StockMovement — immutable, лише фабричний метод. Guid-guard'и
  (ProductId, WarehouseId, CreatedByUserId) винесені в приватний
  EnsureNotEmpty(value, paramName) — DRY-рефакторинг за тим самим
  принципом, що й EnsurePositiveQuantity у StockItem. CreatedAt
  виставляється доменом автоматично (DateTime.UtcNow), не приймається
  як параметр — це унеможливлює підробку часу аудиту через публічний API.
- PurchaseOrder — найскладніший агрегат, очікується найбільше TDD-раундів.
- Оцінка обсягу решти Етапу 1: ~9-11 TDD-раундів загалом.
- Прийнято branching workflow: кожна сутність — окрема feature-гілка,
  merge у main через PR звичайним merge-комітом (не squash), щоб
  зберегти видиму історію TDD-циклів для портфоліо.
- Assertions — стандартний xUnit (`Assert.Equal`, `Assert.Throws` тощо),
  без FluentAssertions/Shouldly. Виявлено розбіжність із початковим
  планом технологій під час написання SkuValueTests — виправлено.
- GitHub-репозиторій створено під акаунтом `PavelPPPP`. Локальна машина
  мала SSH-ключ, прив'язаний до іншого акаунту (`pidlepynskyi-portfolio`)
  — для push під `PavelPPPP` створено окремий SSH-ключ
  (`~/.ssh/id_ed25519_pavelppp`) + host-аліас `github-pavelppp` у
  `~/.ssh/config`. Remote `origin` використовує саме цей аліас
  (`git@github-pavelppp:PavelPPPP/StockFlow.git`), а не звичайний
  `github.com`.
- Git-процес: feature-гілка на кожну сутність → PR → merge-коміт
  (не squash) → видалення гілки і локально, і на remote після merge.
  Перший повний цикл пройдено на `feature/product`.
- PurchaseOrder — стан-машина статусів узгоджена перед стартом TDD:
  `Draft → Sent → PartiallyReceived/Received`; `Cancel()` дозволено лише
  з `Draft`/`Sent` (не з `PartiallyReceived` — товар уже частково на
  складі, повне скасування там суперечило б фактичному стану інвентарю;
  часткове скасування — stretch-фіча `CancelRemaining()`). `Send()`
  вимагає непорожній `Lines[]`. Перший TDD-раунд агрегату — знизу вгору:
  спершу `PurchaseOrderLine` (дочірній обʼєкт), потім `PurchaseOrder.Create()`.
- Структура файлів домену (підтверджено, узгоджується з уже існуючими
  Product/Category/Warehouse/Supplier/StockMovement):
  код — `src/StockFlow.Domain/Entities/<Сутність>.cs`;
  тести — `tests/StockFlow.Domain.UnitTests/Entities/<Сутність>Tests.cs`.
  PurchaseOrderLine: `src/StockFlow.Domain/Entities/PurchaseOrderLine.cs` +
  `tests/StockFlow.Domain.UnitTests/Entities/PurchaseOrderLineTests.cs`.
- Гранулярність TDD-циклу уточнена: один тест = один Red-Green(-Refactor)
  цикл = один окремий коміт-кандидат (не пачка тестів в одному коміті).
  Тести пишуться по одному, послідовно, а не всі одразу наперед —
  інакше втрачається сенс TDD як ітеративного процесу.
- PurchaseOrderLine завершено: `ArgumentException` — для `Guid.Empty`
  (невалідний ідентифікатор як такий), `ArgumentOutOfRangeException` —
  для `quantityOrdered <= 0` і `unitPrice < 0` (значення поза допустимим
  діапазоном) — той самий поділ, що вже використовувався у StockItem/
  StockMovement. `UnitPrice = 0` — валідне значення (окремий контрольний
  тест), на відміну від `quantityOrdered`, де нуль заборонений.
- PurchaseOrder: розподіл відповідальності за Tell-Don't-Ask —
  `PurchaseOrderLine.Receive(quantity)` сам захищає власний інваріант
  (`QuantityReceived + quantity ≤ QuantityOrdered`); `PurchaseOrder`
  не має публічного доступу до зміни `QuantityReceived` напряму
  (сеттер приватний), лише делегує через `Receive()`. `PurchaseOrder`
  відповідає за агрегатні перевірки (Status, existence productId) і
  перерахунок власного Status після делегування.
- Чотири нові кастомні винятки (namespace `StockFlow.Domain.Exceptions`,
  той самий стиль, що InsufficientStockException — контекстне
  повідомлення з параметрами конструктора):
  - `InvalidPurchaseOrderStatusTransitionException(orderId, currentStatus,
    attemptedOperation)` — спільний для Send/Cancel/AddLine/ReceiveLine
    при порушенні дозволених переходів статусу.
  - `EmptyPurchaseOrderException(orderId)` — Send() з порожніми Lines[].
  - `OverReceiptException(productId, requestedQuantity, alreadyReceived,
    quantityOrdered)` — PurchaseOrderLine.Receive() перевищує замовлену
    кількість.
  - `PurchaseOrderLineNotFoundException(orderId, productId)` —
    ReceiveLine() з productId, якого немає серед Lines (замінив
    непрозорий InvalidOperationException від Single()).
- AddLine(): дублікат ProductId — `ArgumentException` (некоректний
  вхідний параметр виклику), а не доменний виняток — за аналогією з
  Guid.Empty-кейсом, не зі станом агрегату.
- Виявлено й виправлено збій процесу: для PurchaseOrder гілку
  змержено й видалено ДО того, як план-файл оновлено патчами —
  довелось комітити docs-оновлення окремо, напряму в main, поза PR.
  Причина: Claude дав команди merge/push раніше, ніж патчі до плану.
  Виправлено на майбутнє (див. розділ "Git-коміти") — docs-коміт
  плану тепер явно йде ДО push/PR, як крок 2 стандартної послідовності.
- Виявлено непослідовність між сесіями: `feature/purchase-order-domain`
  використовував `test:` майже для всіх Green-комітів, включно з тими,
  де змінювався production-код (мало бути `feat:` за визначенням із
  розділу "Git-коміти"). Попередні сесії (`feat:`) були ближчі до
  правильного застосування правила. Історію не переписано (гілка вже
  змержена й видалена) — виправлено застосування правила з наступної
  сутності (`StockTransfer`).
- StockTransfer: стан-машина статусів узгоджена перед стартом TDD —
  `Draft → InTransit → Completed`; `Cancel()` дозволено лише з
  `Draft`/`InTransit`, симетрично до межі "останнього безпечного статусу"
  у PurchaseOrder (`PartiallyReceived` там / `Completed` тут — обидва
  означають, що залишки вже реально змінились, і Cancel звідти
  заборонений). На відміну від PurchaseOrder, часткового скасування
  (`CancelRemaining()`-аналога) для StockTransfer не передбачено —
  повний Cancel покриває MVP-потреби.
- Три нові кастомні винятки для StockTransfer (namespace
  `StockFlow.Domain.Exceptions`, той самий стиль конструктора з
  параметрами-контекстом):
  - `SameWarehouseTransferException(warehouseId)` — FromWarehouseId
    дорівнює ToWarehouseId.
  - `InvalidStockTransferStatusTransitionException(transferId,
    currentStatus, attemptedOperation)` — порушення дозволених переходів
    (включно зі спробою Cancel з Completed).
  - `EmptyStockTransferException(transferId)` — перехід у InTransit з
    порожніми Lines[].
- TDD-порядок для StockTransfer — знизу вгору, як і для PurchaseOrder:
  спершу `StockTransferLine` (дочірній об'єкт, без залежності від
  статусу), потім `StockTransfer.Create()`, потім переходи статусів.
- Виявлено другий випадок пропуску `docs:`-коміту (після StockTransfer-
  патчів Claude не дав команду коміту для MD-файлу одразу, а перейшов
  до інструкцій по тесту) — той самий клас збою, що й з PurchaseOrder,
  але тепер сама команда відсутня, а не порядок команд переплутано.
  Виправлено правило 8 в "Інструкції для Claude": самоперевірка перед
  КОЖНОЮ відповіддю після видачі патчів до плану — чи дана команда
  `docs:`-коміту одразу в тій самій відповіді, а не в наступній.
- Правило 8.e переформульовано вдруге: перша спроба виправлення
  ("Green-код АБО docs-патчі") була вузькою — перелічувала конкретні
  типи замість посилання на вже наявний загальний критерій "завершена
  одиниця роботи" з розділу "🔀 Git-коміти". Фінальне формулювання
  посилається на цей критерій напряму, без переліку типів — так само,
  як пункт 6 (патч-алгоритм) навмисно уникає фіксованого переліку
  розділів. Застосовується без змін на всіх етапах Roadmap (Backend,
  Frontend, DevOps), не лише в контексті Domain-шару.
- Виявлено розбіжність між PurchaseOrderLine (guard у конструкторі) і
  рештою сутностей (guard у Create(), конструктор без логіки).
  Узгоджено: мажоритарний патерн (Create()-валідація) — стандарт,
  PurchaseOrderLine — відхилення, залишене як є заднім числом.
  Заодно узгоджено критерій виносу guard-логіки в окремий приватний
  метод: лише після фактичного дублювання (Rule of Three), не
  заздалегідь — для StockTransferLine.Create() з єдиним ProductId-guard
  перевірка лишається інлайн, без EnsureNotEmpty.
- StockTransfer.AddLine(): дублікат ProductId заборонено, узгоджено за
  прямою аналогією з PurchaseOrder.AddLine() — StockTransferLine не
  має поля, що відрізняло б одну партію товару від іншої (немає
  BatchNumber/LotId), тому два рядки з однаковим ProductId були б
  нерозрізненними й мали б сенс лише як один рядок зі скоригованою
  кількістю. Якщо в майбутньому знадобляться партії — розширення через
  BatchNumber як частину ідентичності рядка, а не послаблення поточного
  інваріанту.
- Виявлено прогалину в дизайні StockTransfer: узгоджена стан-машина
  (Draft→InTransit→Completed) не передбачала окремого значення enum
  для скасованого стану, хоча Cancel() з Draft/InTransit був узгоджений
  раніше. Виправлено додаванням StockTransferStatus.Cancelled —
  за аналогією з тим, як PurchaseOrderStatus має власне значення для
  скасованого стану. Урок: при узгодженні стан-машини (стрілки
  Draft→InTransit→Completed) явно перелічувати ВСІ кінцеві стани,
  включно з "гілками" на кшталт Cancel, а не лише лінійний happy path.
- Узгоджено повну структуру репозиторію (див. окремий розділ "📁 Структура
  репозиторію") — до появи Infrastructure/Api деталей структура була лише
  частково задокументована.
- Валідація Command/Query — через MediatR `ValidationBehavior`
  (pipeline), не ручний виклик у Handler'і. Це і є причина існування
  папок `Common/Behaviors` і `Common/Exceptions` у структурі — вони не
  порожні "про запас", а відображають конкретне рішення.
- `tests/StockFlow.Api.FunctionalTests` поки не створено — з'явиться
  разом із першим контролером (пункт Roadmap "Перший контролер +
  перевірка через Swagger UI"), не раніше.
- Третій випадок пропуску `docs:`-коміту в тій самій відповіді, де
  видано патчі (після PurchaseOrder і StockTransfer) — цього разу в
  контексті інфраструктурного кроку (CQRS/MediatR wiring), не TDD-циклу
  сутності. Підтверджує, що правило 8.e стосується будь-якої "одиниці
  роботи" незалежно від типу (feat/chore/docs-only), а не лише
  завершення домену. Користувач сам зауважив пропуск — самоперевірка
  Claude перед відповіддю мала б це зловити першою.
- Виняток із правила "один тест = один коміт" (розділ "🔀 Git-коміти"):
  для `CreateCategoryCommandValidator` тести `Validate_ParentCategoryIdExists_
  ReturnsNoError` і `Validate_NoParentCategoryId_ReturnsNoError` об'єднано
  в один `test:`-коміт. Обґрунтування: обидва тести з'явились одразу Green
  (validator вже реалізовував цю логіку раніше, жодних Red-Green циклів
  не було) і разом покривають одну логічну пару — "ParentCategoryId
  заданий і існує" / "ParentCategoryId не заданий" для тієї самої
  умовної гілки (`.When(x => x.ParentCategoryId.HasValue)`). Це НЕ
  загальне правило "групувати схожі тести" — лише для випадку, коли
  кілька тестів одночасно Green без проміжного production-коду між
  ними й перевіряють одну нерозривну пару сценаріїв.
- Четвертий випадок пропуску коміту в тій самій відповіді, де видано
  код/патчі — цього разу не docs-коміт (як тричі раніше), а сам
  production-код (`CategoriesController.cs`, `ExceptionHandlingMiddleware.cs`).
  Ризик вищий, ніж із docs: наступна відповідь (патч `Program.cs`) уже
  посилалась на `ExceptionHandlingMiddleware`, ніби він існує в
  репозиторії, хоча коміту для нього ще не було. Підтверджує
  необхідність застосовувати самоперевірку правила 8.e буквально до
  КОЖНОГО виданого коду без винятку, а не лише до docs-патчів чи
  завершення TDD-циклу сутності.
- Перетин меж Етапу 3 / Етапу 4 (жовтень 2026, сесія "CategoriesController
  + EF Core persistence") — свідомий, одноразовий, не прецедент для
  майбутніх етапів. Причина: наскрізна перевірка (контролер + Swagger UI)
  для use case Category вимагала реалізації `ICategoryRepository`/
  `IUnitOfWork`, яка за означенням належить Етапу 4. Після завершення
  `CategoryRepository`/`UnitOfWork`/`ApplicationDbContext` і підтвердження
  робочого Swagger UI — робота повертається строго в межі Етапу 3 (Commands/
  Queries для use cases 2–9), Infrastructure більше не чіпається до
  появи наступного реального запиту на новий репозиторій.
- Виявлено конфлікт між щойно запропонованим рішенням (Варіант Б1/В1 —
  замінити всі `ArgumentException`/`ArgumentOutOfRangeException` на
  єдиний `DomainValidationException`) і вже задокументованим раніше
  (StockItem, PurchaseOrderLine, PurchaseOrder.AddLine — рядки про
  "ArgumentException — невалідний параметр виклику, не бізнес-сценарій")
  принципом свідомого розведення двох категорій помилок. Конфлікт
  виявлено лише після того, як користувач попросив звірити з реальним
  файлом плану, а не покладатись на пам'ять Claude — патерн, що
  повторюється (пор. попередній запис про походження `ValidationException`).
  Вирішено на користь **збереження** вже задокументованого розведення
  (Варіант Д): `DomainException` — лише для 9 іменованих бізнес-винятків,
  прості guard-клаузи лишаються на `ArgumentException`/
  `ArgumentOutOfRangeException` без змін, `ExceptionHandlingMiddleware`
  ловить обидва сімейства окремо.
- Клас `DomainValidationException` (Варіант Б1/В1) — скасовано, у коді
  не використовується, якщо вже встиг бути закомічений — підлягає
  видаленню.

---

## 🧭 Інструкція для Claude (як продовжити в новому чаті)

1. Прочитай "📍 Поточний статус" — це показує, на якому кроці зупинились.
2. Детальний опис у розділі "📦 Доменна модель" ОЗНАЧАЄ ЛИШЕ ПОГОДЖЕНИЙ
   ДИЗАЙН, А НЕ РЕАЛІЗАЦІЮ. Ніколи не вважай клас/сутність написаною чи
   протестованою на основі того, що вона детально описана в цьому розділі.
   Єдине джерело правди щодо реалізації — стовпець "Статус" у таблицях
   сутностей/Value Objects (⬜/🔶/🟡/✅) та розділ "📍 Поточний статус".
   Якщо статус не ✅ — вважай, що коду й тестів не існує, навіть якщо
   опис виглядає завершеним.
3. Продовжуй з першого незавершеного пункту в "Повний план (Roadmap)".
4. Дотримуйся критерію глибини TDD з розділу "Підхід TDD" — не застосовуй
   однаковий рівень тестування до всіх сутностей автоматично.
5. Пояснюй теорію на кожному кроці — це освітній проєкт.
5a. Коли пропонуєш код чи рішення, яке **не випливає напряму** з уже
    узгоджених у плані домовленостей, а береться із зовнішньої типової
    практики (референсний шаблон, поширений патерн спільноти тощо) —
    явно познач це окремою приміткою на кшталт "Походження: ..." із
    посиланням на джерело, якщо воно є. Не подавай запозичений код так,
    ніби він логічно випливає з попередніх рішень проєкту.
6. Після кожного завершеного КРОКУ (не етапу) — сам пропонуй патч, не
   чекаючи прохання. Формуй його за таким алгоритмом, а не за фіксованим
   списком розділів (список розділів з часом змінюється, алгоритм — ні):

   a. Сформулюй одним реченням факт: що саме змінилось (ухвалено рішення /
      написано код / пройдено тест / змінився статус кроку).
   b. Пройди послідовно по КОЖНОМУ розділу файлу (від "🏛 Архітектурні
      рішення" до "📍 Поточний статус") і перевір: чи згадується ця
      сутність/рішення/крок тут хоч якось — у таблиці, чекбоксі, статус-
      мітці, переліку? Не обмежуйся розділами, які редагувались минулого
      разу.
   c. Для КОЖНОГО знайденого місця, де інформація тепер застаріла —
      сформуй окремий патч (Розділ → Дія → Текст). Один факт може вимагати
      3-5 патчів одночасно, це нормально.
   d. Перш ніж надати патчі користувачу, подумки зістав їх між собою: чи
      не залишається суперечності між розділами після застосування (напр.
      Roadmap каже "не розпочато", а таблиця в Доменній моделі — "✅")?
      Якщо так — виправ ще до того, як показати результат.
   e. Якщо є сумнів, чи розділ стосується факту — краще перевірити і
      пропустити свідомо, ніж не перевіряти взагалі.
7. Патч завжди подавай у форматі: **Розділ** (точна назва) →
   **Дія** ("Замінити повністю" / "Вставити після рядка: `<якір>`") →
   **Текст** готовим markdown-блоком для копіювання. Ніколи не редагуй
   файл сам — користувач вставляє текст вручну.
8. Git-супровід — НЕ як довідкова інформація "за потреби", а як
   обов'язковий крок після КОЖНОГО Green (і Refactor, якщо він був),
   без винятків і без очікування, поки користувач сам запитає:

   a. Одразу після Green — дай ПОВНИЙ, готовий до копіювання набір
      команд: `git add <конкретні файли>` + `git commit -m "..."` у
      форматі Conventional Commits. Не описуй словами "не забудь
      закомітити" — завжди сама команда, готова для copy-paste.
   b. Якщо це перший крок роботи над новою сутністю — ПЕРЕД тестом дай
      команду створення гілки (`git checkout -b feature/...`).
   c. Якщо сутність повністю завершена (усі тести Green, план оновлено)
      — сам підкажи послідовність: push гілки → створення PR → merge
      → видалення гілки (локально І на remote).
   d. НІКОЛИ не вважай, що git remote `origin` уже існує/налаштований,
      або що GitHub-репозиторій уже створений, лише тому, що раніше в
      розмові згадувався push. Якщо це перший push у сесії/проєкті —
      спершу запитай (або запропонуй перевірити командою `git remote -v`),
      перш ніж давати `git push`.
   e. Самоперевірка перед КОЖНОЮ відповіддю: чи завершилась у цій
      відповіді хоч одна "одиниця роботи" за критерієм з розділу
      "🔀 Git-коміти" (завершена, компілюється, логічно цілісна сама
      по собі — Green-цикл TDD, non-TDD код, конфіг, міграція, CI-файл,
      патчі до плану, будь-що)? Якщо так — чи дана для неї команда
      коміту в цій ЖЕ відповіді? Критерій навмисно не прив'язаний до
      етапу проєкту чи типу файлу — застосовується однаково на
      Backend/Frontend/DevOps, у цьому чаті й у наступних. Якщо
      команди немає — це помилка, яку треба виправити тут же.
   f. Користувач ще напрацьовує досвід з git — команди завжди повні й
      конкретні (з реальними шляхами до файлів цієї сесії), ніколи не
      "git add ." і ніколи не абстрактний опис без самих команд.