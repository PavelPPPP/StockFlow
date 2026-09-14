namespace StockFlow.Domain.Exceptions
{
    public class OverReceiptException : Exception
    {
        public OverReceiptException(Guid productId, int requestedQuantity, int alreadyReceived, int quantityOrdered)
            : base($"Cannot receive {requestedQuantity} of product {productId}: {alreadyReceived} already received, {quantityOrdered} ordered in total.")
        {
        }
    }
}