namespace StockFlow.Domain.Entities
{
    public class StockTransferLine
    {
        public Guid ProductId { get; }
        public int Quantity { get; }

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

            return new StockTransferLine(productId, quantity);
        }
    }
}