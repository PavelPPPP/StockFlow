using StockFlow.Domain.Common;
using StockFlow.Domain.Enums;

namespace StockFlow.Domain.Entities
{
    public class StockMovement : Entity<Guid>
    {
        public Guid ProductId { get; private set; }
        public Guid WarehouseId { get; private set; }
        public MovementType Type { get; private set; }
        public int Quantity { get; private set; }
        public string? Reason { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public Guid CreatedByUserId { get; private set; }

        private StockMovement(
            Guid id,
            Guid productId,
            Guid warehouseId,
            MovementType type,
            int quantity,
            string? reason,
            Guid createdByUserId,
            DateTime createdAt)
                : base(id)
        {
            ProductId = productId;
            WarehouseId = warehouseId;
            Type = type;
            Quantity = quantity;
            Reason = reason;
            CreatedByUserId = createdByUserId;
            CreatedAt = createdAt;
        }

        public static StockMovement Create(
            Guid productId,
            Guid warehouseId,
            MovementType type,
            int quantity,
            string? reason,
            Guid createdByUserId)
        {
            EnsureNotEmpty(productId, nameof(productId));
            EnsureNotEmpty(warehouseId, nameof(warehouseId));
            EnsureNotEmpty(createdByUserId, nameof(createdByUserId));

            if (quantity <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be positive.");
            }

            return new StockMovement(Guid.NewGuid(), productId, warehouseId, type, quantity, reason, createdByUserId, DateTime.UtcNow);
        }

        private static void EnsureNotEmpty(Guid value, string paramName)
        {
            if (value == Guid.Empty)
            {
                throw new ArgumentException($"{paramName} cannot be empty.", paramName);
            }
        }
    }
}