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

        [Fact]
        public void AddLine_WithValidData_ShouldAddLineToTransfer()
        {
            var transfer = StockTransfer.Create(Guid.NewGuid(), Guid.NewGuid());
            var productId = Guid.NewGuid();

            transfer.AddLine(productId, quantity: 5);

            Assert.Single(transfer.Lines);
            Assert.Equal(productId, transfer.Lines[0].ProductId);
            Assert.Equal(5, transfer.Lines[0].Quantity);
        }

        [Fact]
        public void AddLine_WithDublicateProductId_ShouldThrowAgumentException()
        {
            var transfer = StockTransfer.Create(Guid.NewGuid(), Guid.NewGuid());
            var productId = Guid.NewGuid();
            transfer.AddLine(productId, quantity: 5);

            Assert.Throws<ArgumentException>(() => 
                transfer.AddLine(productId, quantity: 3));
        }

        [Fact]
        public void Ship_WithNonEmptyLines_ShouldChangeStatusToInTransit()
        {
            var transfer = StockTransfer.Create(Guid.NewGuid(), Guid.NewGuid());
            transfer.AddLine(Guid.NewGuid(), quantity: 5);

            transfer.Ship();

            Assert.Equal(StockTransferStatus.InTransit, transfer.Status);
        }

        [Fact]
        public void Ship_WithEmptyLines_ShouldThrowEmptyStockTransferException()
        {
            var transfer = StockTransfer.Create(Guid.NewGuid(), Guid.NewGuid());

            Assert.Throws<EmptyStockTransferException>(() => transfer.Ship());
        }

        [Fact]
        public void Ship_WhenAlreadyInTransit_ShouldThrowInvalidStockTransferStatusTransitionException()
        {
            var transfer = StockTransfer.Create(Guid.NewGuid(), Guid.NewGuid());
            transfer.AddLine(Guid.NewGuid(), quantity: 5);
            transfer.Ship();

            Assert.Throws<InvalidStockTransferStatusTransitionException>(() => transfer.Ship());
        }

        [Fact]
        public void Complete_WhenInTransit_ShouldChangeStatusToCompleted()
        {
            var transfer = StockTransfer.Create(Guid.NewGuid(), Guid.NewGuid());
            transfer.AddLine(Guid.NewGuid(), quantity: 5);
            transfer.Ship();

            transfer.Complete();

            Assert.Equal(StockTransferStatus.Completed, transfer.Status);
        }
    }
}