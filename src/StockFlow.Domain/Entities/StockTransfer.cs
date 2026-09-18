using StockFlow.Domain.Common;
using StockFlow.Domain.Enums;
using StockFlow.Domain.Exceptions;

namespace StockFlow.Domain.Entities
{
    public class StockTransfer : AggregateRoot<Guid>
    {
        public Guid FromWarehouseId { get; }
        public Guid ToWarehouseId { get; }
        public StockTransferStatus Status { get; private set; }
        public DateTime CreatedAt { get; }

        private readonly List<StockTransferLine> _lines = new();
        public IReadOnlyList<StockTransferLine> Lines => _lines.AsReadOnly();

        private StockTransfer(Guid Id, Guid fromWarehouseId, Guid toWarehouseId) 
            : base(Id)
        {
            FromWarehouseId = fromWarehouseId;
            ToWarehouseId = toWarehouseId;
            Status = StockTransferStatus.Draft;
            CreatedAt = DateTime.UtcNow;
        }

        public static StockTransfer Create(Guid fromWarehouseId, Guid toWarehouseId)
        {
            if (fromWarehouseId == toWarehouseId)
            {
                throw new SameWarehouseTransferException(fromWarehouseId);
            }

            return new StockTransfer(Guid.NewGuid(), fromWarehouseId, toWarehouseId);
        }

        public void AddLine(Guid productId, int quantity)
        {
            if (_lines.Any(l => l.ProductId == productId))
            {
                throw new ArgumentException($"Product {productId} is already added to this transfer.", nameof(productId));
            }

            var line = StockTransferLine.Create(productId, quantity);
            _lines.Add(line);
        }

        public void Ship()
        {
            if (Status != StockTransferStatus.Draft)
            {
                throw new InvalidStockTransferStatusTransitionException(Id, Status, nameof(Ship));
            }

            if (_lines.Count == 0)
            {
                throw new EmptyStockTransferException(Id);
            }

            Status = StockTransferStatus.InTransit;
        }

        public void Complete()
        {
            if (Status != StockTransferStatus.InTransit)
            {
                throw new InvalidStockTransferStatusTransitionException(Id, Status, nameof(Complete));
            }

            Status = StockTransferStatus.Completed;
        }
    }
}