using StockFlow.Domain.Common;
using StockFlow.Domain.Enums;
using StockFlow.Domain.Exceptions;

namespace StockFlow.Domain.Entities
{
    public class PurchaseOrder : AggregateRoot<Guid>
    {
        private readonly List<PurchaseOrderLine> _lines = new();

        public Guid SupplierId { get; }
        public Guid WarehouseId { get; }
        public PurchaseOrderStatus Status { get; private set; }
        public DateTime OrderDate { get; }
        public DateTime? ExpectedDeliveryDate { get; private set; }
        public IReadOnlyList<PurchaseOrderLine> Lines => _lines.AsReadOnly();

        private PurchaseOrder(Guid supplierId, Guid warehouseId, DateTime? expectedDeliveryDate)
            : base(Guid.NewGuid())
        {
            if (supplierId == Guid.Empty)
            {
                throw new ArgumentException("SuppliedId cannot be empty.", nameof(supplierId));
            }

            if (warehouseId == Guid.Empty)
            {
                throw new ArgumentException("WarehouseId cannot be empty.", nameof(warehouseId));
            }

            SupplierId = supplierId;
            WarehouseId = warehouseId;
            ExpectedDeliveryDate = expectedDeliveryDate;
            Status = PurchaseOrderStatus.Draft;
            OrderDate = DateTime.UtcNow;
        }

        public static PurchaseOrder Create(Guid supplierId, Guid warehouseId, DateTime? expectedDeliveryDate)
        {
            return new PurchaseOrder(supplierId, warehouseId, expectedDeliveryDate);
        }

        public void AddLine(Guid productId, int quantityOrdered, decimal unitPrice)
        {
            var line = PurchaseOrderLine.Create(productId, quantityOrdered, unitPrice);
            _lines.Add(line);
        }

        public void Send()
        {
            if (Status != PurchaseOrderStatus.Draft)
            {
                throw new InvalidPurchaseOrderStatusTransactionException(Id, Status, nameof(Send));
            }

            if (_lines.Count == 0)
            {
                throw new EmptyPurchaseOrderException(Id);
            }

            Status = PurchaseOrderStatus.Sent;
        }
    }
}