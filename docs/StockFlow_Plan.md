# StockFlow — План і дизайн

> Стабільний документ: змінюється лише коли ухвалюється/змінюється рішення
> чи контракт. Поточний стан виконання — у **StockFlow_Progress.md**.

---

## 🎯 Мета проєкту

Fullstack pet-проєкт для портфоліо рівня Middle:
- C#/.NET на бекенді з сучасною архітектурою
- Впевнений React на фронтенді (закрити прогалину у Frontend)
- TDD (вибірково), DDD, CQRS, Clean Architecture
- DevOps (Docker, CI/CD)

Часовий горизонт: 3+ місяці. Тема: Warehouse / Inventory Management System.

---

## 🛠 Технологічний стек

**Backend:** ASP.NET Core Web API (.NET 9), Clean Architecture
(Domain/Application/Infrastructure/API), CQRS + MediatR, EF Core + PostgreSQL,
FluentValidation, ASP.NET Identity + JWT (access + refresh), Serilog, Hangfire.
- Тести: xUnit + NSubstitute; стандартний `Assert.*` (без FluentAssertions).
- Testcontainers (інтеграційні тести з реальною PostgreSQL).
- Swashbuckle (`Swashbuckle.AspNetCore`) для Swagger UI; вбудований
  `Microsoft.AspNetCore.OpenApi` видалено.
- WebApplicationFactory (functional-тести критичних наскрізних сценаріїв).
- Postman (ручна колекція для демонстрації API).

**Frontend:** React + TypeScript + Vite, TanStack Query, Zustand,
React Hook Form + Zod, MUI, Vitest + React Testing Library.

**DevOps:** Docker + docker-compose, GitHub Actions, деплой Render/Railway
(backend+DB) + Vercel/Netlify (frontend).

---

## 🏛 Архітектурні рішення

1. **Clean Architecture** — Domain без залежностей (жодного зовнішнього
   NuGet); Application залежить лише від Domain; Infrastructure/API — через
   інтерфейси.
2. **CQRS + MediatR** — кожен use case = окремий Command/Query + Handler.
3. **DDD (тактичні патерни)** — Entity, Value Object, Aggregate Root, Domain
   Events, Domain Service (за потреби).
4. **TDD — вибірково**, за ціною помилки (див. "Підхід до тестування").
5. **Repository + Unit of Work.** Правило: один Repository — один Aggregate
   Root **або незалежно персистована проста сутність**. `Category`,
   `Warehouse`, `Supplier` — не Aggregate Root, але кожна має власний
   `DbSet` і власний репозиторій (не generic "на все"). Дочірні об'єкти
   (`PurchaseOrderLine`) власного репозиторію не мають.
6. **Dependency Inversion** — інтерфейси репозиторіїв в Application,
   реалізація в Infrastructure, wiring у API (Program.cs).
7. **Value Objects — `record`**, приватний ctor, статична фабрика `Create()`
   з валідацією, властивості лише `get` (без `init`), без positional-синтаксису
   для полів з валідацією (блокує обхід через `with`).
8. **Guard-логіка в Entity** — у статичній фабриці `Create()`; приватний
   конструктор лише присвоює поля. Виняток-відхилення: `PurchaseOrderLine`
   (guard у конструкторі, залишено як є).
9. **Application — feature-based (vertical slice).** Папка на сутність
   (`Categories/`, `Products/`, `Warehouses/`, `Suppliers/`, `StockItems/`,
   `PurchaseOrders/`, `StockTransfers/`), всередині `Commands/<UseCase>/` і
   `Queries/<UseCase>/`; для Command — `<UseCase>Command.cs`,
   `<UseCase>CommandHandler.cs`, `<UseCase>CommandValidator.cs`.
   Крос-каттинг — у `Common/` (`Interfaces/`, `Behaviors/`, `Exceptions/`).
10. **Валідація через MediatR pipeline** (`ValidationBehavior<TRequest,
    TResponse>`), не ручний виклик у Handler'і. При помилках кидається
    `StockFlow.Application.Common.Exceptions.ValidationException` (власний,
    не з FluentValidation). Обмеження generic-параметра — `where TRequest :
    notnull` (а не `IRequest<TResponse>`, бо воно тихо виключало команди
    `IRequest` без `<T>`).
11. **Ієрархія винятків (Варіант Д).** `DomainException` (abstract) — лише для
    9 іменованих бізнес-правил. Прості guard-клаузи лишаються на
    `ArgumentException`/`ArgumentOutOfRangeException` (невалідний вхідний
    параметр ≠ бізнес-правило). `DomainValidationException` скасовано.
    `ExceptionHandlingMiddleware` має 3 catch-блоки, усі → `400`:
    `ValidationException` ("Validation failed"), `DomainException`
    ("Business rule violation"), `ArgumentException` ("Invalid argument").
12. **Де живуть перевірки.** Те, що сутність перевіряє про себе без БД
    (`Category` не власний батько — `CategorySelfParentException`) → Domain.
    Умови, що залежать від інших записів (існування батька, наявність дітей
    перед видаленням, унікальність SKU) → FluentValidation-валідатор через
    репозиторій, результат `ValidationException`. Глибокі цикли категорій
    не перевіряються (YAGNI). Category — hard delete.
13. **Product — soft delete (`IsActive`).** На Product посилаються
    StockItem, PurchaseOrderLine, StockTransferLine, StockMovement (audit
    log): hard delete або блокувався б FK, або знищував історію. Методи
    `Deactivate()`/`Activate()` ідемпотентні. Фізичного Delete-use-case
    немає — `DeactivateProductCommand`. Список за замовчуванням — лише
    активні (`includeInactive` для решти). SKU незмінний після створення
    (`UpdateDetails` його не приймає), унікальність — валідатор + унікальний
    індекс у БД.
14. **Тонкий `IUnitOfWork`** (лише `SaveChangesAsync`), репозиторії
    інжектяться в Handler окремо; НЕ "товстий" UoW з властивостями-
    репозиторіями (Open/Closed, YAGNI). *Походження:* стандартний DDD-патерн
    (Evans, Fowler); свідоме відхилення від jasontaylordev/CleanArchitecture,
    де репозиторіїв/UoW немає.
15. **Handler не змінює два агрегати "одним махом"** через прямі посилання;
    кожен оновлюється окремо, узгодженість — Unit of Work.

---

## 📁 Структура репозиторію

```
StockFlow/
├── .gitignore, README.md, docker-compose.yml, StockFlow.sln
├── docs/                         ← StockFlow_Instructions.md, StockFlow_Plan.md, StockFlow_Progress.md
├── src/
│   ├── StockFlow.Domain/         (Common/, Entities/, ValueObjects/, Enums/, Events/, Exceptions/)
│   ├── StockFlow.Application/    (DependencyInjection.cs, Common/{Interfaces,Behaviors,Exceptions}/,
│   │                              Products/, Categories/, Warehouses/, Suppliers/,
│   │                              StockItems/, PurchaseOrders/, StockTransfers/)
│   ├── StockFlow.Infrastructure/ (Persistence/{Configurations,Migrations,Repositories}/,
│   │                              Identity/, BackgroundJobs/, DependencyInjection.cs)
│   └── StockFlow.Api/            (Controllers/, Middleware/, Program.cs, appsettings.json)
├── tests/
│   ├── StockFlow.Domain.UnitTests/         (Entities/<Сутність>Tests.cs)
│   ├── StockFlow.Application.UnitTests/    (<Feature>/Commands/<UseCase>/<UseCase>CommandValidatorTests.cs)
│   ├── StockFlow.Infrastructure.IntegrationTests/
│   └── StockFlow.Api.FunctionalTests/      (з'явиться з першим функціональним тестом)
└── client/                       ← Етап 6 (React)
```

Код домену: `src/StockFlow.Domain/Entities/<Сутність>.cs`; тести:
`tests/StockFlow.Domain.UnitTests/Entities/<Сутність>Tests.cs`.
`DependencyInjection.cs` — один файл у корені кожного проєкту.

---

## 🧱 Базові класи

**Domain/Common** — `AggregateRoot<TId>` (`src/StockFlow.Domain/Common/AggregateRoot.cs`):

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

- `TId` = `Guid` для всіх сутностей. `Id` передається через `base(id)`, власне
  поле `Id` у нащадку не оголошується.
- Метод називається `RiseDomainEvent` (опечатка "Rise" зафіксована як є, не
  виправляти без окремого рефакторинг-рішення).
**`Entity<TId>`** (`src/StockFlow.Domain/Common/Entity.cs`):

```csharp
namespace StockFlow.Domain.Common
{
    public abstract class Entity<TId>
    {
        public TId Id { get; protected set; } = default!;

        protected Entity() { }

        protected Entity(TId id) => Id = id;

        public override bool Equals(object? obj)
        {
            if (obj is not Entity<TId> other) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id!.Equals(other.Id);
        }

        public override int GetHashCode() => Id!.GetHashCode();
    }
}
```

**`IDomainEvent`** (`src/StockFlow.Domain/Common/IDomainEvent.cs`):

```csharp
namespace StockFlow.Domain.Common
{
    public interface IDomainEvent
    {
        DateTime OccurredOn { get; }
    }
}
```

Примітка: `Equals` порівнює лише `Id` і не перевіряє конкретний тип, тому
`Product` і `Category` з однаковим Guid вважались би рівними. На практиці
Guid не збігаються, тож зміни не потрібні (YAGNI); це відома особливість.

**Application/Common**
- `ValidationBehavior<TRequest,TResponse>` — перехоплює кожен Command/Query,
  запускає всі `IValidator<TRequest>`, при помилках кидає `ValidationException`.
  Реєстрація: `cfg.AddOpenBehavior(typeof(ValidationBehavior<,>))`.
- `ValidationException` — `Errors: IDictionary<string, string[]>` по
  `PropertyName` (форма сумісна з `ValidationProblemDetails`). *Походження:*
  типовий шаблон jasontaylordev/CleanArchitecture.

**Ієрархія винятків**
```
Exception
├── ArgumentException / ArgumentOutOfRangeException   (прості guard-клаузи)
└── DomainException (abstract, Domain/Exceptions/DomainException.cs)
    ├── InsufficientStockException
    ├── InsufficientAvailableStockException
    ├── OverReceiptException
    ├── EmptyPurchaseOrderException
    ├── EmptyStockTransferException
    ├── InvalidPurchaseOrderStatusTransitionException
    ├── InvalidStockTransferStatusTransitionException
    ├── PurchaseOrderLineNotFoundException
    ├── SameWarehouseTransferException
    └── CategorySelfParentException
```
(Рішення 12 додало `CategorySelfParentException : DomainException` — вона
10-та; у рішенні 11 згадувалось 9, бо створена пізніше.)

**Інтерфейси репозиторіїв (Application/Common/Interfaces)**
- `ICategoryRepository`: `ExistsAsync`, `HasChildrenAsync`, `GetByIdAsync`
  (nullable), `AddAsync`, `Remove` (синхронний — лише change tracker).
- `IUnitOfWork`: `SaveChangesAsync(CancellationToken)`.
- Для Products: `IProductRepository` — з'явиться в `feature/create-product-command`
  (додати сигнатури сюди після погодження).

---

## 📦 Доменна модель (дизайн)

### Сутності

| Сутність | Тип | Ключові поля | Особливості |
|---|---|---|---|
| Product | Entity<Guid> | Id, SKU (SkuValue), Name, Description, CategoryId, UnitOfMeasure, Barcode?, IsActive | Create/UpdateDetails з guard-ами; Name — Trim(); SKU незмінний; Deactivate/Activate ідемпотентні |
| Category | Entity<Guid> | Id, Name, ParentCategoryId? | Self-reference; Name — Trim(); перевірка існування батька — Application |
| Warehouse | Entity<Guid> | Id, Name, Address, IsActive | IsActive=true за замовчуванням; Activate/Deactivate з'являться в use case CRUD складів |
| Supplier | Entity<Guid> | Id, Name, ContactEmail (EmailValue), Phone | Name/Phone — Trim() + guard; Phone — простий string |
| StockItem | AggregateRoot | ProductId, WarehouseId, QuantityOnHand, QuantityReserved, MinimumStockLevel | Create/Receive/Deduct/Reserve/ReleaseReservation |
| StockMovement | immutable | Id, ProductId, WarehouseId, Type, Quantity, Reason, CreatedAt, CreatedByUserId | CreatedAt = UtcNow, виставляє домен |
| PurchaseOrder | AggregateRoot | SupplierId, WarehouseId, Status, OrderDate, ExpectedDeliveryDate, Lines | Draft→Sent→PartiallyReceived/Received; Cancel з Draft/Sent |
| PurchaseOrderLine | дочірній | ProductId, QuantityOrdered, QuantityReceived, UnitPrice | `Receive()` сам захищає інваріант (Tell-Don't-Ask); UnitPrice ≥ 0 |
| StockTransfer | AggregateRoot | FromWarehouseId, ToWarehouseId, Status, Lines, CreatedAt | Draft→InTransit→Completed; Cancel з Draft/InTransit |
| StockTransferLine | дочірній | ProductId, Quantity | ProductId≠Empty, Quantity>0 |
| ApplicationUser | ASP.NET Identity | — | Ролі: Admin, Manager, WarehouseWorker |

### Value Objects та enum'и

- `SkuValue` — record, `^[A-Z0-9-]+$`, довжина 3–30.
- `EmailValue` — record, спрощений regex.
- `UnitOfMeasure`: Pcs, Kg, L, M. `MovementType`: Receipt, Issue, Transfer,
  Adjustment. `StockTransferStatus`: Draft, InTransit, Completed, Cancelled.
  `PurchaseOrderStatus`: Draft, Sent, PartiallyReceived, Received, Cancelled.
- Barcode — `string?` без валідації (кандидат на VO, якщо з'явиться сканер).
- `Money` не існує й не вводиться (YAGNI): ціна — `decimal UnitPrice`.

### Aggregate Roots (межі узгодженості)

StockItem (StockMovement — дочірній immutable-запис), Product, PurchaseOrder
(+Lines), StockTransfer (+Lines). Category/Warehouse/Supplier — прості
довідники (не AR, але з власними репозиторіями).

### Бізнес-правила

- `QuantityOnHand` ≥ 0; `QuantityReserved` ≤ `QuantityOnHand`.
- Залишок змінюється лише через створення `StockMovement`.
- SKU унікальний у системі (перевірка в Application) і незмінний.
- Product не видаляється фізично; для неактивного товару не можна створювати
  нові `StockItem`/`PurchaseOrderLine`/`StockTransferLine` (перевірка — у
  відповідних use case).
- PurchaseOrder: отримання не перевищує замовлене (`OverReceiptException`);
  `Send()` вимагає непорожні Lines; `Cancel()` лише з Draft/Sent (часткове
  скасування — stretch `CancelRemaining()`); `AddLine` — дублікат ProductId
  → `ArgumentException`.
- StockTransfer: `From != To`; `Ship` вимагає непорожні Lines; `Cancel()` лише
  з Draft/InTransit; дублікат ProductId заборонено (`ArgumentException`);
  партії/лоти не підтримуються на MVP (розширення — через `BatchNumber` як
  частину ідентичності рядка).
- `QuantityOnHand < MinimumStockLevel` → `LowStockDetectedEvent`.
- PurchaseOrder створюють лише Admin/Manager; WarehouseWorker лише реєструє рух.

### Use cases (MVP)

1. CRUD товарів і категорій
2. CRUD складів
3. CRUD постачальників
4. Ручна реєстрація руху товару
5. Purchase Order: створення → отримання → оновлення залишку
6. Переміщення між складами
7. Дашборд залишків
8. Сповіщення про низькі залишки
9. Audit log руху з фільтрами
10. Автентифікація + рольовий доступ

**Stretch:** звітність (оборотність, вартість запасів), SignalR, ABC-аналіз.

### Доменні події

`StockReceivedEvent`, `StockIssuedEvent`, `StockTransferredEvent`,
`LowStockDetectedEvent`, `PurchaseOrderCreatedEvent`, `PurchaseOrderReceivedEvent`

---

## 🧪 Підхід до тестування

**Критерій глибини TDD — ціна помилки.** Висока (арифметика, гроші, залишки)
→ повне покриття з `[Theory]` (StockItem). Низька (прості перевірки) → базові
`[Fact]` (Product). Виняток — специфічна логіка (напр. нормалізація) отримує
більше уваги навіть у простій сутності.

**TDD так:** поведінка AR з інваріантами; Handler'и з розгалуженнями;
складні валідатори; Zod-схеми; нетривіальні хуки.
**TDD ні:** прості CRUD; EF-конфігурації й репозиторії (Testcontainers після
стабілізації контракту); "вітринні" компоненти.

**DAMP у тестах, DRY у production.** Guard виноситься в helper після
дублювання у 2+ місцях (`EnsurePositiveQuantity` у StockItem,
`EnsureNotEmpty` у StockMovement).

**Піраміда (4 рівні):**
1. Unit (Domain, Application) — TDD.
2. Integration (Infrastructure) — Testcontainers.
3. Functional/API (WebApplicationFactory) — лише критичні наскрізні сценарії
   (PurchaseOrder receive → StockItem, 401 без токена, атомарність
   StockTransfer); не для простих CRUD.
4. Manual (Swagger UI, Postman) — Postman-колекція експортується наприкінці.

**Pipeline-тести:** для кожного нового виду запиту — тест через справжній
`IMediator` з `AddApplication()` (еталон:
`ValidationBehaviorTests.Send_NonGenericRequest_RunsValidationBehavior`;
потрібні `NullLoggerFactory` і пакет `Microsoft.Extensions.DependencyInjection`).
Ручна перевірка негативних сценаріїв у Swagger — обов'язкова частина
завершення use case.

---

## 🔀 Git-правила

- Гілка на сутність/крок: `feature/<назва>` від актуального `main`.
- Коміт — на межах TDD-циклу (після Green; Refactor — окремо, якщо суттєвий).
  Один тест = один цикл = один коміт.
- Без TDD (базові класи, EF-конфіги, DI, CRUD без правил): коміт, коли
  одиниця роботи завершена, компілюється й цілісна.
- Conventional Commits: `feat:` (змінився production-код), `test:` (лише
  тест-файл), `refactor:`, `fix:`, `chore:`, `docs:`.
- Виняток-прецедент: два тести, що одночасно Green без проміжного
  production-коду й покривають нерозривну пару сценаріїв, можна об'єднати
  в один `test:`-коміт (не загальне правило).
- PR → merge-коміт (не squash), щоб було видно історію TDD-циклів.
- Порядок завершення: Green-коміт → docs-коміт у тій самій гілці → push →
  PR/merge → видалення гілки (локально й на remote).
- GitHub: акаунт `PavelPPPP`, remote `git@github-pavelppp:PavelPPPP/StockFlow.git`
  (SSH-ключ `~/.ssh/id_ed25519_pavelppp`, host-аліас у `~/.ssh/config`).

---

## 🗺 Roadmap

**Етап 0+2 — Підготовка й архітектура рішення:** репозиторій, solution,
4 backend-проєкти + тестові, посилання між шарами, структура папок,
.gitignore/README, теорія Clean/Onion та DIP.

**Етап 1 — Моделювання домену:** Common-класи; StockItem; Product (+SkuValue,
UnitOfMeasure); Category; Warehouse; EmailValue + Supplier; MovementType +
StockMovement; PurchaseOrder + Line; StockTransfer + Line; теорія DDD.

**Етап 3 — Backend: ядро логіки (CQRS + MediatR):** теорія CQRS; структура
Application; MediatR + FluentValidation + ValidationBehavior; Commands/Queries
для use cases 1–9; перший контролер + ExceptionHandlingMiddleware;
`tests/StockFlow.Api.FunctionalTests`.
- Use case 1 Categories: Create, GetById, Update, Delete.
- Use case 2 Products — порядок гілок: `feature/product-domain-update` →
  `feature/create-product-command` (IProductRepository, ProductConfiguration,
  міграція, Create) → `feature/get-product-by-id-query` →
  `feature/get-products-query` (пагінація, фільтр за категорією/пошуком,
  `includeInactive`) → `feature/update-product-command` →
  `feature/deactivate-product-command`.
- Use cases 3–9: Warehouses, Suppliers, StockItems/ручний рух, PurchaseOrders,
  StockTransfers, дашборд, low-stock, audit log.

**Етап 4 — Персистентність:** DbContext, конфігурації, міграції,
Repository/UoW; інтеграційні тести Testcontainers; теорія Repository, N+1.
*(Для Category частково виконано раніше за графіком — див. Progress.)*

**Етап 5 — Автентифікація:** JWT + refresh, ролі, policy-based авторизація,
Swagger Security Definition, functional-тест 401, теорія JWT/RBAC vs claims.

**Етап 6 — Frontend: база:** Vite, роутинг, feature-based структура,
TanStack Query; теорія кешування/інвалідації.

**Етап 7 — Frontend: інтерфейс:** списки, форми (RHF+Zod), дашборд залишків,
тести компонентів з логікою; теорія контрольованих форм, оптимістичних оновлень.

**Етап 8 — Тестування, DevOps, деплой:** Postman-колекція, Docker +
docker-compose, GitHub Actions (усі автоматизовані рівні тестів), деплой,
теорія піраміди тестування й CI/CD.

**Stretch:** SignalR live-оновлення; звітність / ABC-аналіз.

---

## 📝 Журнал дизайн-рішень (скорочено)

- Value Objects через `record` — компілятор дає equality/ToString/Deconstruct;
  приватний ctor + фабрика блокує обхід валідації.
- `UnitOfMeasure` — enum (закритий набір), не VO.
- `ContactEmail` — VO за зразком SkuValue; `Phone` — string (формат надто
  варіативний для MVP).
- `StockMovement.CreatedAt` не приймається ззовні — захист аудиту від підробки.
- `PurchaseOrderLine.Receive()` — Tell-Don't-Ask; `QuantityReceived` має
  приватний сеттер.
- `UnitPrice = 0` валідний, `quantityOrdered = 0` — ні.
- `StockTransferStatus.Cancelled` додано під час TDD (прогалина в початковій
  стан-машині). Урок: при узгодженні стан-машини перелічувати ВСІ кінцеві
  стани, не лише happy path.
- Свідомий перетин меж Етапу 3/4 (жовтень 2026): для наскрізної перевірки
  Category реалізовано `CategoryRepository`/`UnitOfWork`/`ApplicationDbContext`.
  Одноразово, не прецедент: далі Infrastructure чіпається лише коли новий
  use case потребує нового репозиторію.
- Реальний запуск до реалізації Infrastructure падав на DI (fail-fast) — це
  була очікувана поведінка; Варіант А (чекати Етап 4) обрано замість
  in-memory заглушки.
- Конвенція винятків для guard-клауз: `ArgumentException` — невалідний
  ідентифікатор (`Guid.Empty`); `ArgumentOutOfRangeException` — значення поза
  діапазоном (`quantity <= 0`, `unitPrice < 0`). Бізнес-сценарії (нестача
  залишку тощо) — окремі `DomainException`-нащадки з контекстним повідомленням.
- Сигнатури конструкторів доменних винятків (контекст у параметрах):
  - `InvalidPurchaseOrderStatusTransitionException(orderId, currentStatus, attemptedOperation)`
  - `EmptyPurchaseOrderException(orderId)`
  - `OverReceiptException(productId, requestedQuantity, alreadyReceived, quantityOrdered)`
  - `PurchaseOrderLineNotFoundException(orderId, productId)`
  - `SameWarehouseTransferException(warehouseId)`
  - `InvalidStockTransferStatusTransitionException(transferId, currentStatus, attemptedOperation)`
  - `EmptyStockTransferException(transferId)`
- `IsActive` у `Warehouse` не приймається параметром `Create()` — домен
  виставляє `true` сам; `Warehouse.Name`/`Address` нормалізуються через Trim().
- PurchaseOrder/StockTransfer розроблялись знизу вгору: спершу дочірній
  об'єкт (Line), потім агрегат, потім переходи статусів.