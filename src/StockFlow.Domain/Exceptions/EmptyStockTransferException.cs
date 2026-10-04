namespace StockFlow.Domain.Exceptions
{
    public class EmptyStockTransferException : DomainException
    {
        public EmptyStockTransferException(Guid transferId)
            : base($"Cannot ship StockTransfer {transferId} without any lines.") 
        {
        }
    }
}