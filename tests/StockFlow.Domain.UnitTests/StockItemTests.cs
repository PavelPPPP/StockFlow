using StockFlow.Domain.Entities;
using StockFlow.Domain.Exceptions;
using Xunit;

namespace StockFlow.Domain.UnitTests
{
    public class StockItemTests
    {
        [Fact]
        public void Receive_WhenCalledWithPositiveQuantity_IncreasesQuantityOnHand()
        {
            // Arrange
            var stockItem = StockItem.Create(
                productId: Guid.NewGuid(),
                warehouseId: Guid.NewGuid(),
                minimumStockLevel: 10);

            // Act
            stockItem.Receive(50);

            // Assert
            Assert.Equal(50, stockItem.QuantityOnHand);
        }

        [Fact]
        public void Deduct_WhenQuantityIsSufficient_ReducesQuantityOnHand()
        {
            // Arrange
            var stockItem = StockItem.Create(
                productId: Guid.NewGuid(),
                warehouseId: Guid.NewGuid(),
                minimumStockLevel: 10);
            stockItem.Receive(20);

            // Act
            stockItem.Deduct(5);

            // Assert
            Assert.Equal(15, stockItem.QuantityOnHand);
        }

        [Fact]
        public void Deduct_WhenQuantityIsSufficient_ThrowInsufficientStockException()
        {
            // Arrange
            var stockItem = StockItem.Create(
                productId: Guid.NewGuid(),
                warehouseId: Guid.NewGuid(),
                minimumStockLevel: 10);

            // Act, Assert
            Assert.Throws<InsufficientStockException>(() =>
            {
                stockItem.Deduct(15);
            });
        }

        [Fact]
        public void Reserve_WhenCalledWithPositiveQuantity_IncreasesQuantityReserved()
        {
            // Arrange
            var stockItem = StockItem.Create(
                productId: Guid.NewGuid(),
                warehouseId: Guid.NewGuid(),
                minimumStockLevel: 10);
            stockItem.Receive(20);

            // Act
            stockItem.Reserve(10);

            // Assert
            Assert.Equal(10, stockItem.QuantityReserved);
        }

        [Fact]
        public void Reserve_WhenQuantityExceedsAvailable_ThrowInsufficientAvailableStockException()
        {
            // Arrange
            var stockItem = StockItem.Create(
                productId: Guid.NewGuid(),
                warehouseId: Guid.NewGuid(),
                minimumStockLevel: 10);
            stockItem.Receive(20);
            stockItem.Reserve(10);

            // Act, Assert
            Assert.Throws<InsufficientAvailableStockException>(() =>
            {
                stockItem.Reserve(15);
            });
        }

        [Fact]
        public void ReleaseReservation_WhenCalledWithPositiveQuantity_ReducesQuantityReserved()
        {
            // Arrange
            var stockItem = StockItem.Create(
                productId: Guid.NewGuid(),
                warehouseId: Guid.NewGuid(),
                minimumStockLevel: 10);
            stockItem.Receive(20);
            stockItem.Reserve(10);

            // Act
            stockItem.ReleaseReservation(10);

            // Assert
            Assert.Equal(0, stockItem.QuantityReserved);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-5)]
        public void Receive_WhenQuantityIsNotPositive_ThrowsArgumentOutOfRangeException(int invalidQuantity)
        {
            // Arrange
            var stockItem = StockItem.Create(
                productId: Guid.NewGuid(),
                warehouseId: Guid.NewGuid(),
                minimumStockLevel: 10);

            // Act, Assert
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                stockItem.Receive(invalidQuantity);
            });
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-5)]
        public void Deduct_WhenQuantityIsNotPositive_ThrowsArgumentOutOfRangeException(int invalidQuantity)
        {
            // Arrange
            var stockItem = StockItem.Create(
                productId: Guid.NewGuid(),
                warehouseId: Guid.NewGuid(),
                minimumStockLevel: 10);

            // Act, Assert
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                stockItem.Deduct(invalidQuantity);
            });
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-5)]
        public void Reserve_WhenQuantityIsNotPositive_ThrowsArgumentOutOfRangeException(int invalidQuantity)
        {
            // Arrange
            var stockItem = StockItem.Create(
                productId: Guid.NewGuid(),
                warehouseId: Guid.NewGuid(),
                minimumStockLevel: 10);

            // Act, Assert
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                stockItem.Reserve(invalidQuantity);
            });
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-5)]
        public void ReleaseReservation_WhenQuantityIsNotPositive_ThrowsArgumentOutOfRangeException(int invalidQuantity)
        {
            // Arrange
            var stockItem = StockItem.Create(
                productId: Guid.NewGuid(),
                warehouseId: Guid.NewGuid(),
                minimumStockLevel: 10);

            // Act, Assert
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                stockItem.ReleaseReservation(invalidQuantity);
            });
        }
    }
}