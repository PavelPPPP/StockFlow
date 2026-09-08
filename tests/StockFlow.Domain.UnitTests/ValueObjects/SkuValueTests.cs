using StockFlow.Domain.ValueObjects;

namespace StockFlow.Domain.UnitTests.ValueObjects
{
    public class SkuValueTests
    {
        [Fact]
        public void Create_WithValidSku_ShouldSucceed()
        {
            var sku = SkuValue.Create("ABC-123");

            Assert.Equal("ABC-123", sku.Value);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Create_WithNullOrEmptySku_ShouldThrow(string invalidValue)
        {
            Assert.Throws<ArgumentException>(() => SkuValue.Create(invalidValue));
        }

        [Theory]
        [InlineData("AB")]      // 2 characters — fewer than the minimum
        [InlineData("A1234567890123456789012345678901")]        // 31+ characters — more than the maximum
        public void Create_WithInvalidLength_ShouldThrow(string invalidValue)
        {
            Assert.Throws<ArgumentException>(() => SkuValue.Create(invalidValue));
        }

        [Theory]
        [InlineData("abc-123")]     // lowercase letters
        [InlineData("ABC 123")]     // space inside
        [InlineData("ABC_123")]     // an underline instead of a hyphen
        [InlineData("АБВ-123")]     // Cyrillic alphabet
        public void Create_WithInvalidCharacters_ShouldThrow(string invalidValue)
        {
            Assert.Throws<ArgumentException>(() => SkuValue.Create(invalidValue));
        }

        [Fact]
        public void Create_TwoSkusWithSameValue_ShouldBeEqual()
        {
            var sku1 = SkuValue.Create("ABC-123");
            var sku2 = SkuValue.Create("ABC-123");

            Assert.Equal(sku1, sku2);
            Assert.True(sku1 == sku2);
            Assert.Equal(sku1.GetHashCode(), sku2.GetHashCode());
        }
    }
}