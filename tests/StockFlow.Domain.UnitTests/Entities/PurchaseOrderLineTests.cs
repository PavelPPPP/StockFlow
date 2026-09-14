using StockFlow.Domain.Entities;

namespace StockFlow.Domain.UnitTests.Entities
{
    public class PurchaseOrderLineTests
    {
        [Fact]
        public void Create_WithValidData_ShouldSucceed()
        {
            var productId = Guid.NewGuid();

            var line = PurchaseOrderLine.Create(productId, quantityOrdered: 10, unitPrice: 25.50m);

            Assert.Equal(productId, line.ProductId);
            Assert.Equal(10, line.QuantityOrdered);
            Assert.Equal(0, line.QuantityReceived);
            Assert.Equal(25.50m, line.UnitPrice);
        }

        [Fact]
        public void Create_WithEmptyProductId_ShouldThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(() => 
                PurchaseOrderLine.Create(Guid.Empty, quantityOrdered: 10, unitPrice: 25.50m));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-5)]
        public void Create_WithNonPositiveQuantityOrdered_ShouldThrowArgumentOutOfRangeException(int invalidQuantity)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => 
                PurchaseOrderLine.Create(Guid.NewGuid(), quantityOrdered: invalidQuantity, unitPrice: 25.50m));
        }

        [Fact]
        public void Create_WithNegativeUnitPrice_ShouldThrowArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                PurchaseOrderLine.Create(Guid.NewGuid(), quantityOrdered: 10, unitPrice: -1m));
        }

        [Fact]
        public void Create_WithZeroUnitPrice_ShouldSucceed()
        {
            var line = PurchaseOrderLine.Create(Guid.NewGuid(), quantityOrdered: 10, unitPrice: 0m);

            Assert.Equal(0m, line.UnitPrice);
        }

        [Fact]
        public void Receive_ValidQuantity_ShouldIncreaseQantityReceived()
        {
            var line = PurchaseOrderLine.Create(Guid.NewGuid(), quantityOrdered: 10, unitPrice: 25.50m);

            line.Receive(4);

            Assert.Equal(4, line.QuantityReceived);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-3)]
        public void Receive_NonPositiveQuantity_ShouldArgumentOutOfRangeException(int invalidQuantity)
        {
            var line = PurchaseOrderLine.Create(Guid.NewGuid(), quantityOrdered: 10, unitPrice: 25.50m);

            Assert.Throws<ArgumentOutOfRangeException>(() => line.Receive(invalidQuantity));
        }
    }
}