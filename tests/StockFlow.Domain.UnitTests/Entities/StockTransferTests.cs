using StockFlow.Domain.Entities;
using StockFlow.Domain.Enums;

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
    }
}