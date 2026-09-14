using StockFlow.Domain.Entities;
using StockFlow.Domain.Enums;
using StockFlow.Domain.Exceptions;

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

        [Fact]
        public void AddLine_ToDraftOrder_ShouldAddLineToCollection()
        {
            var order = PurchaseOrder.Create(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow.AddDays(7));
            var productId = Guid.NewGuid();

            order.AddLine(productId, quantityOrdered: 5, unitPrice: 10m);

            Assert.Single(order.Lines);
            Assert.Equal(productId, order.Lines[0].ProductId);
            Assert.Equal(5, order.Lines[0].QuantityOrdered);
        }

        [Fact]
        public void Send_DraftOrderWithLines_ShouldChangeStatusToSent()
        {
            var order = PurchaseOrder.Create(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow.AddDays(7));
            order.AddLine(Guid.NewGuid(), quantityOrdered: 5, unitPrice: 10m);

            order.Send();

            Assert.Equal(PurchaseOrderStatus.Sent, order.Status);
        }

        [Fact]
        public void Send_AlreadySentOrder_ShouldThrowInvalidPurchaseOrderStatusTransactionException()
        {
            var order = PurchaseOrder.Create(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow.AddDays(7));
            order.AddLine(Guid.NewGuid(), quantityOrdered: 5, unitPrice: 10m);
            order.Send();

            Assert.Throws<InvalidPurchaseOrderStatusTransactionException>(() => order.Send());
        }

        [Fact]
        public void Send_DraftOrderWithoutLines_ShouldThrowEmptyPurchaseOrderException()
        {
            var order = PurchaseOrder.Create(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow.AddDays(7));

            Assert.Throws<EmptyPurchaseOrderException>(() => order.Send());
        }

        [Fact]
        public void AddLine_ToSentOrder_ShouldThrowInvalidPurchaseOrderStatusTransactionException()
        {
            var order = PurchaseOrder.Create(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow.AddDays(7));
            order.AddLine(Guid.NewGuid(), quantityOrdered: 5, unitPrice: 10m);
            order.Send();

            Assert.Throws<InvalidPurchaseOrderStatusTransactionException>(() => 
                order.AddLine(Guid.NewGuid(), quantityOrdered: 3, unitPrice: 8m));
        }

        [Fact]
        public void AddLine_WithDublicateProductId_ShouldThrowArgumentException()
        {
            var order = PurchaseOrder.Create(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow.AddDays(7));
            var productId = Guid.NewGuid();
            order.AddLine(productId, quantityOrdered: 5, unitPrice: 10m);

            Assert.Throws<ArgumentException>(() => 
                order.AddLine(productId, quantityOrdered: 3, unitPrice: 8m));
        }
    }
}