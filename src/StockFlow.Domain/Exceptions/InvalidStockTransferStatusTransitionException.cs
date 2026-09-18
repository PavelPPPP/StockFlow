using StockFlow.Domain.Enums;

namespace StockFlow.Domain.Exceptions
{
    public class InvalidStockTransferStatusTransitionException : Exception
    {
        public InvalidStockTransferStatusTransitionException(Guid transferId, StockTransferStatus currentStatus, string attemptedOperation) 
            : base($"Cannot perform '{attemptedOperation}' on StockTranfer {transferId} while its status is '{currentStatus}'") 
        {
        }
    }
}