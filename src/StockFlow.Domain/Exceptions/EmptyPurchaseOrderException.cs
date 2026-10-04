namespace StockFlow.Domain.Exceptions
{
    public class EmptyPurchaseOrderException : DomainException
    {
        public EmptyPurchaseOrderException(Guid orderId)
            : base($"Cannot send purchase order {orderId}: order has no lines.")
        { 
        }
    }
}