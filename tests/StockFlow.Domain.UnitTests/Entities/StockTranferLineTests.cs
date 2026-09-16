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
    }
}