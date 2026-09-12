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
    }
}