using StockFlow.Domain.Common;
using StockFlow.Domain.Exceptions;

namespace StockFlow.Domain.Entities
{
    public class StockItem : AggregateRoot<Guid>
    {
        public Guid ProductId { get; private set; }
        public Guid WarehouseId { get; private set; }
        public int QuantityOnHand { get; private set; }
        public int QuantityReserved { get; private set; }
        public int MinimumStockLevel { get; private set; }

        // EF Core потребує приватний беспараметричний конструктор — додамо його пізніше,
        // коли дійдемо до Infrastructure. Зараз навмисно не додаємо зайвого.
        private StockItem(Guid id, Guid productId, Guid warehouseId, int minimumStockLevel)
            : base(id)
        {
            ProductId = productId;
            WarehouseId = warehouseId;
            MinimumStockLevel = minimumStockLevel;
            QuantityOnHand = 0;
            QuantityReserved = 0;
        }

        public static StockItem Create(Guid productId, Guid warehouseId, int minimumStockLevel)
        {
            return new StockItem(Guid.NewGuid(), productId, warehouseId, minimumStockLevel);
        }

        public void Receive(int quantity)
        {
            EnsurePositiveQuantity(quantity, nameof(quantity));
            QuantityOnHand += quantity;
        }

        public void Deduct(int quantity)
        {
            EnsurePositiveQuantity(quantity, nameof(quantity));
            if (quantity > QuantityOnHand)
            {
                throw new InsufficientStockException(ProductId, quantity, QuantityOnHand);
            }

            QuantityOnHand -= quantity;
        }

        public void Reserve(int quantity)
        {
            EnsurePositiveQuantity(quantity, nameof(quantity));
            
            var available = QuantityOnHand - QuantityReserved;

            if (quantity > available)
            {
                throw new InsufficientAvailableStockException(ProductId, quantity, available);
            }

            QuantityReserved += quantity;
        }

        public void ReleaseReservation(int quantity)
        {
            EnsurePositiveQuantity(quantity, nameof(quantity));
            QuantityReserved -= quantity;
        }

        private static void EnsurePositiveQuantity(int quantity, string paramName)
        {
            if (quantity <= 0)
            {
                throw new ArgumentOutOfRangeException(paramName, quantity, "Quantity must be greated than zero.");
            }
        }
    }
}