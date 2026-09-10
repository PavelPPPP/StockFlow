using StockFlow.Domain.Entities;
using StockFlow.Domain.Enums;
using StockFlow.Domain.ValueObjects;

namespace StockFlow.Domain.UnitTests.Entities
{
    public class ProductTests
    {
        [Fact]
        public void Create_WithValidData_ShouldSucceed()
        {
            var sku = SkuValue.Create("ABC-123");
            var categoryId = Guid.NewGuid();
            var product = Product.Create(
                sku, 
                "Test Product", 
                "Description", 
                categoryId, 
                UnitOfMeasure.Pcs, 
                barcode: null);

            Assert.Equal(sku, product.Sku);
            Assert.Equal("Test Product", product.Name);
            Assert.Equal(categoryId, product.CategoryId);
            Assert.Equal(UnitOfMeasure.Pcs, product.UnitOfMeasure);
            Assert.Null(product.Barcode);
        }

        [Fact]
        public void Create_WithEmptyName_ShouldThrow()
        {
            var sku = SkuValue.Create("ABC-123");

            Assert.Throws<ArgumentException>(() => 
                Product.Create(sku, "", "Description", Guid.NewGuid(), UnitOfMeasure.Pcs, null));
        }

        [Fact]
        public void Create_WithEmptyCategoryId_ShouldThrow()
        {
            var sku = SkuValue.Create("ABC-123");

            Assert.Throws<ArgumentException>(() =>
                Product.Create(sku, "Test Product", "Description", Guid.Empty, UnitOfMeasure.Pcs, null));
        }
    }
}
