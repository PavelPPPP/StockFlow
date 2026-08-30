namespace StockFlow.Domain.Exceptions
{
    public class InsufficientAvailableStockException : Exception
    {
        public InsufficientAvailableStockException(Guid productId, int requested, int available)
            : base($"Insufficient available stock for product {productId}: requested {requested}, but only {available} available for reservation.")
        {
        }
    }
}