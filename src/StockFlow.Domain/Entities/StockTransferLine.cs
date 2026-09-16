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
            return new StockTransferLine(productId, quantity);
        }
    }
}