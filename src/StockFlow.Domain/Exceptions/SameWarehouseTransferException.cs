namespace StockFlow.Domain.Exceptions
{
    public class SameWarehouseTransferException : DomainException
    {
        public SameWarehouseTransferException(Guid warehouseId)
            : base($"Cannot transfer stock within the same warehouse (WarehouseId: {warehouseId}).") 
        {
        }
    }
}