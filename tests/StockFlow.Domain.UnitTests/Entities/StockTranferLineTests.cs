using StockFlow.Domain.Entities;

namespace StockFlow.Domain.UnitTests.Entities
{
    public class StockTranferLineTests
    {
        [Fact]
        public void Create_WithValidData_ShouldSucceed()
        {
            var productId = Guid.NewGuid();

            var line = StockTransferLine.Create(productId, quantity: 10);

            Assert.Equal(productId, line.ProductId);
            Assert.Equal(10, line.Quantity);
        }

        [Fact]
        public void Create_WithEmptyProductId_ShouldThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(() => 
                StockTransferLine.Create(Guid.Empty, quantity: 1));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Create_WithInvalidQuantity_ShouldThrowArgumentOutOfRangeException(int invalidQuantity)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => 
                StockTransferLine.Create(Guid.NewGuid(), invalidQuantity));
        }
    }
}