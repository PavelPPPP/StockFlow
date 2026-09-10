using StockFlow.Domain.Entities;

namespace StockFlow.Domain.UnitTests.Entities
{
    public class WarehouseTests
    {
        [Fact]
        public void Create_WithValidData_ShouldSucceedAndBeActiveByDefault()
        {
            // Arrange
            var name = "Main Warehouse";
            var address = "123 Industrial Rd";

            // Act
            var warehouse = Warehouse.Create(name, address);

            // Assert
            Assert.Equal(name, warehouse.Name);
            Assert.Equal(address, warehouse.Address);
            Assert.True(warehouse.IsActive);
        }

        [Fact]
        public void Create_WithEmptyAddress_ShouldThrouArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => Warehouse.Create("Main Warehouse", string.Empty));
        }

        [Fact]
        public void Create_WithNameContainingLeadingAndTrailingWhitespace_ShouldTrimName()
        {
            // Arrange
            var nameWithWhitespace = "  Main Warehouse  ";

            // Act
            var warehouse = Warehouse.Create(nameWithWhitespace, "123 Industrial Rd");

            // Assert
            Assert.Equal("Main Warehouse", warehouse.Name);
        }

        [Fact]
        public void Create_WithAddressContainingLeadingAndTrailingWhitespace_ShouldTrimAddress()
        {
            // Arrange
            var addressWithWhitespace = "   123 Industrial Rd   ";

            // Act
            var warehous = Warehouse.Create("Main Warehouse", addressWithWhitespace);

            // Assert
            Assert.Equal("123 Industrial Rd", warehous.Address);
        }
    }
}