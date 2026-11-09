# StockFlow — Прогрес

> Змінний документ: оновлюється після кожного завершеного кроку.
> Дизайн і рішення — у **StockFlow_Plan.md**.
> Остання звірка з файлом "StockFlow Plan.md": 31.10.2026. Пункти з позначкою
> ❓ потребують підтвердження користувача (див. "Відкриті питання").

**Легенда:** ⬜ не розпочато · 🔶 дизайн погоджено, коду немає · 🟡 TDD в процесі · ✅ реалізовано й протестовано

---

## 📍 Поточний крок

**Гілка `feature/create-product-command` (в процесі).** Зроблено:
`IProductRepository`. Далі: `CreateProductCommand` + валідатор (TDD по одному
тесту), Handler, Infrastructure (`DbSet<Product>`, `ProductConfiguration`,
`ProductRepository`, DI, міграція), `ProductsController`, pipeline-тест,
ручна перевірка негативних сценаріїв у Swagger.

Стан: Етап 1 завершено; Етап 3 — use case 1 (Categories) закрито, use case 2
(Products): доменна частина змерджена в `main`, Application/Infrastructure
розпочато.

---

## 📦 Статус сутностей

| Сутність / VO | Статус | Тести | Примітка |
|---|---|---|---|
| StockItem (AR) | ✅ | повне, з `[Theory]` | Create/Receive/Deduct/Reserve/ReleaseReservation |
| Product | ✅ | 12 | `feature/product-domain-update` змерджено |
| Category | ✅ | 4 (+ UpdateDetails) | Є CQRS-use cases і репозиторій |
| Warehouse | ✅ | 5 | Activate/Deactivate ще не написані (з'являться в CRUD складів) |
| Supplier | ✅ | — | Create з guard-ами та Trim |
| StockMovement | ✅ | 7 | |
| PurchaseOrder | ✅ | 16 | |
| PurchaseOrderLine | ✅ | 10 | |
| StockTransfer (AR) | ✅ | 13 | `feature/stock-transfer-domain` змерджено |
| StockTransferLine | ✅ | 3 | |
| SkuValue | ✅ | 5 | |
| EmailValue | ✅ | 8 | |
| UnitOfMeasure, MovementType | ✅ | — | enum'и |
| StockTransferStatus | ✅ | — | (у старому файлі помилково 🔶) |
| ApplicationUser | ⬜ | — | Етап 5 |

---

## 🧩 Статус Application / Infrastructure / API

| Елемент | Статус | Примітка |
|---|---|---|
| MediatR + FluentValidation, `AddApplication()` | ✅ | |
| `ValidationBehavior` + `ValidationException` | ✅ | Виправлено баг `IRequest` без `<T>` (`where TRequest : notnull`) |
| `DomainException` + 9 іменованих винятків | ✅ | + `CategorySelfParentException` |
| `ExceptionHandlingMiddleware` (3 catch → 400) | ✅ | |
| `CreateCategoryCommand` | ✅ | Validator 4 тести, Handler 1 |
| `GetCategoryByIdQuery` | ✅ | Handler 2 тести, `CategoryDto`, 404 для неіснуючого Id |
| `UpdateCategoryCommand` | ✅ | Validator 4, Handler 1, `PUT` → 204 |
| `DeleteCategoryCommand` | ✅ | Перевірено в Swagger, включно з негативними сценаріями |
| `CategoriesController` | ✅ | Create/GetById/Update/Delete |
| `ICategoryRepository`, `IUnitOfWork` | ✅ | (у старому файлі 🔶 — застаріло) |
| `CategoryRepository`, `UnitOfWork`, `ApplicationDbContext` | ✅ | Реалізовано в перетині Етапів 3/4; міграція Category застосована, Swagger запускається |
| `ValidationBehaviorTests.Send_NonGenericRequest_RunsValidationBehavior` | ✅ | Еталон pipeline-тесту |
| Products: `IProductRepository`, `ProductConfiguration`, міграція | ⬜ | Наступна гілка |
| Warehouses / Suppliers / StockItems / PurchaseOrders / StockTransfers use cases | ⬜ | Use cases 2–9 |
| `StockFlow.Api.FunctionalTests` | ⬜ | Ще не створено |

---

## ✅ Roadmap-чекліст

### Етап 0 + 2 — Підготовка та архітектура
- [x] Репозиторій, solution, 4 backend-проєкти, посилання між шарами
- [x] Тестові проєкти (xUnit), структура папок, .gitignore, README, перший коміт
- [x] Доменна модель затверджена; теорія Clean/Onion, DIP

### Етап 1 — Моделювання домену (завершено)
- [x] Common: `Entity<TId>`, `AggregateRoot<TId>`, `IDomainEvent`
- [x] StockItem; Product (+SkuValue, UnitOfMeasure); Category; Warehouse
- [x] EmailValue + Supplier; MovementType + StockMovement
- [x] PurchaseOrder + PurchaseOrderLine; StockTransfer + StockTransferLine
- [x] Теорія DDD (Bounded Context, Ubiquitous Language, Aggregate, Domain Service/Event)

### Етап 3 — Backend: ядро логіки
- [x] Теорія CQRS/Mediator/Command vs Query
- [x] Структура Application, MediatR + FluentValidation, ValidationBehavior
- [x] Use case 1: CRUD категорій (Create, GetById, Update, Delete)
- [x] Перший контролер + ExceptionHandlingMiddleware + Swashbuckle
- [ ] Use case 2: Products
  - [x] `feature/product-domain-update` (UpdateDetails, Trim, Entity<Guid>, IsActive/Deactivate/Activate) — змерджено
  - [ ] `feature/create-product-command`
  - [ ] `feature/get-product-by-id-query`
  - [ ] `feature/get-products-query` (пагінація, фільтри, `includeInactive`)
  - [ ] `feature/update-product-command`
  - [ ] `feature/deactivate-product-command`
- [ ] Use cases 3–9 (Warehouses, Suppliers, ручний рух, PurchaseOrders, StockTransfers, дашборд/low-stock, audit log)
- [ ] `tests/StockFlow.Api.FunctionalTests` — перші тести критичних сценаріїв

### Етап 4 — Персистентність
- [~] Часткове: DbContext/Repository/UoW для Category
- [ ] Решта конфігурацій, міграцій, репозиторіїв (по мірі use cases)
- [ ] Інтеграційні тести Testcontainers
- [ ] Теорія: Repository, N+1

### Етапи 5–8 — не розпочато
- [ ] Етап 5: JWT + refresh, ролі, Swagger Security, functional-тест 401
- [ ] Етап 6: Vite, роутинг, TanStack Query
- [ ] Етап 7: списки, форми RHF+Zod, дашборд, тести компонентів
- [ ] Етап 8: Postman, Docker, GitHub Actions, деплой

### Stretch
- [ ] SignalR live-оновлення
- [ ] Звітність / ABC-аналіз

---

## ❓ Відкриті питання

1. Після мерджу `create-product-command` — гілка
   `feature/category-delete-products-guard`: категорія з товарами не
   видаляється (`DeleteCategoryCommandValidator` + `IProductRepository.AnyInCategoryAsync`).
   Інакше FK `Restrict` дасть 500 замість 400.
2. У `feature/get-product-by-id-query` замінити тимчасовий `Created(...)` у
   `ProductsController.Create` на `CreatedAtAction`.

---

## 🗒 Журнал подій (стислий)

- Етап 1 завершено: усі сутності домену реалізовані.
- Варіант Д (ієрархія винятків) узгоджено й реалізовано; Б1/В1 скасовано.
- Use case 1 (Categories) закрито повністю; знайдено й виправлено баг
  `ValidationBehavior` (виявлено лише ручним тестом негативного сценарію Delete).
- Узгоджено дизайн Products (soft delete, незмінний SKU).
- `feature/product-domain-update`: виправлено 5 розбіжностей коду з планом
  (private set; `nameof(categoryId)`; guard-и в Create/UpdateDetails; наслідування
  `Entity<Guid>`; Trim для Name) + `IsActive`/`Deactivate`/`Activate`.

## 📚 Уроки процесу (дублюються в Instructions як правила)

- Docs-коміт плану — ДО push/PR (пропущено для PurchaseOrder).
- Команда коміту — в тій самій відповіді, де видано код/патчі (пропущено 4 рази).
- `feat:` vs `test:` залежить лише від зміни production-коду (в
  `feature/purchase-order-domain` застосовувалось непослідовно; історію
  не переписано).
- Перед новим use case — звіряти реальний код сутності з планом.
- Перед новим архітектурним рішенням — звіряти з уже ухваленими (конфлікт
  Б1/В1 виявлено лише після звірки з файлом).
