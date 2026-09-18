namespace StockFlow.Domain.Exceptions
{
    public class SameWarehouseTransferException : Exception
    {
        public SameWarehouseTransferException(Guid warehouseId)
            : base($"Cannot transfer stock within the same warehouse (WarehouseId: {warehouseId}).") 
        {
        }
    }
}