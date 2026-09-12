namespace StockFlow.Domain.Entities
{
    public class PurchaseOrderLine
    {
        public Guid ProductId { get; }
        public int QuantityOrdered { get; }
        public int QuantityReceived { get; private set; }
        public decimal UnitPrice { get; }

        private PurchaseOrderLine(Guid productId, int quantityOrdered, decimal unitPrice)
        {
            if (productId == Guid.Empty)
            {
                throw new ArgumentException("ProductId cannot be empty.", nameof(productId));
            }

            if (quantityOrdered <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(quantityOrdered), "QuantityOrdered must be positive.");
            }

            if (unitPrice < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(unitPrice), "UnitPrice cannot be negative.");
            }

            ProductId = productId;
            QuantityOrdered = quantityOrdered;
            UnitPrice = unitPrice;
            QuantityReceived = 0;
        }

        public static PurchaseOrderLine Create(Guid productId, int quantityOrdered, decimal unitPrice)
        {
            return new PurchaseOrderLine(productId, quantityOrdered, unitPrice);
        }
    }
}