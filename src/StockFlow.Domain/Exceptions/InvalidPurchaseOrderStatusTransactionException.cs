using StockFlow.Domain.Enums;

namespace StockFlow.Domain.Exceptions
{
    public class InvalidPurchaseOrderStatusTransactionException : Exception
    {
        public InvalidPurchaseOrderStatusTransactionException(Guid orderId, PurchaseOrderStatus currentStatus, string attemptedOperation)
            : base($"Cannot perform '{attemptedOperation}' on purchase order {orderId}: current status is {currentStatus}.")
        {
        }
    }
}