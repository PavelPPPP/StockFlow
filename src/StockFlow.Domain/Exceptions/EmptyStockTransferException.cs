namespace StockFlow.Domain.Exceptions
{
    public class EmptyStockTransferException : Exception
    {
        public EmptyStockTransferException(Guid transferId)
            : base($"Cannot ship StockTransfer {transferId} without any lines.") 
        {
        }
    }
}