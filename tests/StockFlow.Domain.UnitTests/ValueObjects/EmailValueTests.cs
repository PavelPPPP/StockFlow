using StockFlow.Domain.ValueObjects;

namespace StockFlow.Domain.UnitTests.ValueObjects
{
    public class EmailValueTests
    {
        [Fact]
        public void Create_WithValidEmail_ShouldSucceed()
        {
            // Arrange
            var email = "supplier@example.com";

            // Act
            var emailValue = EmailValue.Create(email);

            // Assert
            Assert.Equal(email, emailValue.Value);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Creat_WithNullOrEmptyEmail_ShouldThrowArgumentException(string invalidEmail)
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => EmailValue.Create(invalidEmail));
        }

        [Theory]
        [InlineData("not-an-email")]
        [InlineData("missing-domain@")]
        [InlineData("@missing-local.com")]
        [InlineData("spaces in@email.com")]
        [InlineData("double@@example.com")]
        public void Create_WithInvalidFormat_ShouldThrowArgumentException(string invalidEmail)
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => EmailValue.Create(invalidEmail));
        }

        [Fact]
        public void TwoEmailValues_WithSameValue_ShouldBeEqual()
        {
            // Arrange
            var email1 = EmailValue.Create("supplier@example.com");
            var email2 = EmailValue.Create("supplier@example.com");

            // Act & Assert
            Assert.Equal(email1, email2);
        }
    }
}