using StockFlow.Domain.Entities;
using StockFlow.Domain.Enums;

namespace StockFlow.Domain.UnitTests.Entities
{
    public class StockMovementTests
    {
        [Fact]
        public void Create_WithValidData_ShouldSucceed()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var warehouseId = Guid.NewGuid();
            var type = MovementType.Receipt;
            var quantity = 50;
            var reason = "Initial stock";
            var createdByUserId = Guid.NewGuid();
            var beforeCreate = DateTime.UtcNow;

            // Act
            var movement = StockMovement.Create(productId, warehouseId, type, quantity, reason, createdByUserId);

            // Assert
            Assert.Equal(productId, movement.ProductId);
            Assert.Equal(warehouseId, movement.WarehouseId);
            Assert.Equal(type, movement.Type);
            Assert.Equal(quantity, movement.Quantity);
            Assert.Equal(reason, movement.Reason);
            Assert.Equal(createdByUserId, movement.CreatedByUserId);
            Assert.True(movement.CreatedAt >= beforeCreate && movement.CreatedAt <= DateTime.UtcNow);
        }

        [Fact]
        public void Create_WithNullReason_ShouldSucceed()
        {
            // Act
            var movement = StockMovement.Create(
                Guid.NewGuid(), Guid.NewGuid(), MovementType.Adjastment, 10, reason: null, Guid.NewGuid());

            // Assert
            Assert.Null(movement.Reason);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public void Create_WithNonPositiveQuantity_ShouldThrowArgumentOutOfRangeException(int invalidQuantity)
        {
            // Act && Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => 
                StockMovement.Create(Guid.NewGuid(), Guid.NewGuid(), MovementType.Issue, invalidQuantity, null, Guid.NewGuid()));
        }

        [Fact]
        public void Create_WithEmptyProductId_ShouldThrowArgumentException()
        {
            // Act && Assert
            Assert.Throws<ArgumentException>(() =>
                StockMovement.Create(Guid.Empty, Guid.NewGuid(), MovementType.Receipt, 10, null, Guid.NewGuid()));
        }

        [Fact]
        public void Create_WithEmptyWarehouseId_ShouldThrowArgumentException()
        {
            // Act && Assert
            Assert.Throws<ArgumentException>(() =>
                StockMovement.Create(Guid.NewGuid(), Guid.Empty, MovementType.Receipt, 10, null, Guid.NewGuid()));
        }

        [Fact]
        public void Create_WithEmptyCreatedByUserId_ShouldThrowArgumentException()
        {
            // Act && Assert
            Assert.Throws<ArgumentException>(() =>
                StockMovement.Create(Guid.NewGuid(), Guid.NewGuid(), MovementType.Receipt, 10, null, Guid.Empty));
        }
    }
}