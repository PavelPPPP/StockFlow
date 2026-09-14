namespace StockFlow.Domain.Exceptions
{
    public class EmptyPurchaseOrderException : Exception
    {
        public EmptyPurchaseOrderException(Guid orderId)
            : base($"Cannot send purchase order {orderId}: order has no lines.")
        { 
        }
    }
}