using StockFlow.Domain.Entities;
using StockFlow.Domain.Enums;

namespace StockFlow.Domain.UnitTests.Entities
{
    public class PurchaseOrderTests
    {
        [Fact]
        public void Create_WithValidData_ShouldSucceedAsDraft()
        {
            var supplierId = Guid.NewGuid();
            var warehouseId = Guid.NewGuid();
            var expectedDeliveryDate = DateTime.UtcNow.AddDays(7);

            var order = PurchaseOrder.Create(supplierId, warehouseId, expectedDeliveryDate);

            Assert.Equal(supplierId, order.SupplierId);
            Assert.Equal(warehouseId, order.WarehouseId);
            Assert.Equal(PurchaseOrderStatus.Draft, order.Status);
            Assert.Equal(expectedDeliveryDate, order.ExpectedDeliveryDate);
            Assert.Empty(order.Lines);
            Assert.True(order.OrderDate <= DateTime.UtcNow);
        }

        [Fact]
        public void Create_WithEmptySuppliedId_ShouldThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(() => 
            PurchaseOrder.Create(Guid.Empty, Guid.NewGuid(), DateTime.UtcNow.AddDays(7)));
        }

        [Fact]
        public void Create_WithEmptyWarehousId_ShouldThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
            PurchaseOrder.Create(Guid.NewGuid(), Guid.Empty, DateTime.UtcNow.AddDays(7)));
        }
    }
}