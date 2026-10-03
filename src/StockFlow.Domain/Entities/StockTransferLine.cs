namespace StockFlow.Domain.Entities
{
    public class StockTransferLine
    {
        public Guid ProductId { get; }
        public int Quantity { get; }

        private StockTransferLine() { }
        private StockTransferLine(Guid productId, int quantity)
        {
            ProductId = productId;
            Quantity = quantity;
        }

        public static StockTransferLine Create(Guid productId, int quantity)
        {
            if (productId == Guid.Empty)
            {
                throw new ArgumentException("ProductId cannot be empty.", nameof(productId));
            }

            if (quantity <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be positive.");
            }

            return new StockTransferLine(productId, quantity);
        }
    }
}