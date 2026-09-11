using StockFlow.Domain.Entities;
using StockFlow.Domain.ValueObjects;

namespace StockFlow.Domain.UnitTests.Entities
{
    public class SupplierTests
    {
        [Fact]
        public void Create_WithValidData_ShouldSucceed()
        {
            // Arrange
            var name = "Acme Supplier";
            var email = EmailValue.Create("contact@acme.com");
            var phone = "+1-555-0100";

            // Act
            var supplier = Supplier.Create(name, email, phone);

            // Assert
            Assert.Equal(name, supplier.Name);
            Assert.Equal(email, supplier.ContactEmail);
            Assert.Equal(phone, supplier.Phone);
        }

        [Fact]
        public void Create_WithEmptyName_ShouldThrowArgumentException()
        {
            // Arrange
            var email = EmailValue.Create("contact@acme.com");

            // Act & Assert
            Assert.Throws<ArgumentException>(() => Supplier.Create(string.Empty, email, "+1-555-0100"));
        }

        [Fact]
        public void Create_WithEmptyPhone_ShouldThrowArgumentException()
        {
            // Arrange
            var email = EmailValue.Create("contact@acme.com");

            // Act & Assert
            Assert.Throws<ArgumentException>(() => Supplier.Create("Acme Supplier", email, string.Empty));
        }

        [Fact]
        public void Create_WithNameContainingWitespace_ShouldTrimName()
        {
            // Arrange
            var email = EmailValue.Create("contact@acme.com");

            // Act
            var supplier = Supplier.Create("   Acme Supplier   ", email, "+1-555-0100");

            // Assert
            Assert.Equal("Acme Supplier", supplier.Name);
        }

        [Fact]
        public void Create_WithPhoneContainingWitespace_ShouldTrimPhone()
        {
            // Arrange
            var email = EmailValue.Create("contact@acme.com");

            // Act
            var supplier = Supplier.Create("Acme Supplier", email, "  +1-555-0100  ");

            // Assert
            Assert.Equal("+1-555-0100", supplier.Phone);
        }
    }
}