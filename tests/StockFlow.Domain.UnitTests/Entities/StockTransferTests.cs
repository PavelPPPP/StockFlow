using StockFlow.Domain.Entities;
using StockFlow.Domain.Enums;
using StockFlow.Domain.Exceptions;

namespace StockFlow.Domain.UnitTests.Entities
{
    public class StockTransferTests
    {
        [Fact]
        public void Create_WithValidData_ShouldSucceed()
        {
            var fromWarehouseId = Guid.NewGuid();
            var toWarehouseId = Guid.NewGuid();

            var transfer = StockTransfer.Create(fromWarehouseId, toWarehouseId);

            Assert.Equal(fromWarehouseId, transfer.FromWarehouseId);
            Assert.Equal(toWarehouseId, transfer.ToWarehouseId);
            Assert.Equal(StockTransferStatus.Draft, transfer.Status);
            Assert.Empty(transfer.Lines);
        }

        [Fact]
        public void Create_WithSameFromAndToWarehouseId_ShouldThrowSameWarehouseTransferException()
        {
            var warehouseId = Guid.NewGuid();

            Assert.Throws<SameWarehouseTransferException>(() =>
                StockTransfer.Create(warehouseId, warehouseId));
        }
    }
}