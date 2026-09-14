namespace StockFlow.Domain.Exceptions
{
    public class PurchaseOrderLineNotFoundException : Exception
    {
        public PurchaseOrderLineNotFoundException(Guid orderId, Guid productId)
            : base($"Purchase order {orderId} does not contain a line for product {productId}.")
        {
        }
    }
}