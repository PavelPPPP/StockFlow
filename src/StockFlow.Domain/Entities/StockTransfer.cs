using StockFlow.Domain.Common;
using StockFlow.Domain.Enums;
using StockFlow.Domain.Exceptions;

namespace StockFlow.Domain.Entities
{
    public class StockTransfer : AggregateRoot<Guid>
    {
        public Guid FromWarehouseId { get; }
        public Guid ToWarehouseId { get; }
        public StockTransferStatus Status { get; }
        public DateTime CreatedAt { get; }

        private readonly List<StockTransferLine> _lines = new();
        public IReadOnlyCollection<StockTransferLine> Lines => _lines.AsReadOnly();

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
    }
}