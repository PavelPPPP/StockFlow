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

        [Fact]
        public void UpdateDatails_ValidArguments_UpdatesAllFieldsExceptSku()
        {
            var sku = SkuValue.Create("SKU-001");
            var product = Product.Create(sku, "Old name", "Old description",
                Guid.NewGuid(), UnitOfMeasure.Pcs, "111");
            var newCategoryId = Guid.NewGuid();

            product.UpdateDetails("New name", "New description",
                newCategoryId, UnitOfMeasure.Kg, "222");

            Assert.Equal("New name", product.Name);
            Assert.Equal("New description", product.Description);
            Assert.Equal(newCategoryId, product.CategoryId);
            Assert.Equal(UnitOfMeasure.Kg, product.UnitOfMeasure);
            Assert.Equal("222", product.Barcode);
            Assert.Equal(sku, product.Sku);
        }

        [Fact]
        public void UpdateDetails_WithEmptyName_ShouldThrow()
        {
            var product = Product.Create(
                SkuValue.Create("ABC-123"),
                "Test Product",
                "Description",
                Guid.NewGuid(),
                UnitOfMeasure.Pcs,
                null);

            Assert.Throws<ArgumentException>(() => 
                product.UpdateDetails("", "Description", Guid.NewGuid(), UnitOfMeasure.Pcs, null));
        }

        [Fact]
        public void UpdateDetails_WithEmptyCategoryId_ShouldThrow()
        {
            var product = Product.Create(
                SkuValue.Create("ABC-123"),
                "Test Product",
                "Description",
                Guid.NewGuid(),
                UnitOfMeasure.Pcs,
                null);

            Assert.Throws<ArgumentException>(() =>
                product.UpdateDetails("New name", "Description", Guid.Empty, UnitOfMeasure.Pcs, null));
        }

        [Fact]
        public void Create_WithEmptyCategoryId_ShouldReportCategoryIdAsParamName()
        {
            var sku = SkuValue.Create("ABC-123");

            var exception = Assert.Throws<ArgumentException>(() =>
                Product.Create(sku, "Test Product", "Description", Guid.Empty, UnitOfMeasure.Pcs, null));

            Assert.Equal("categoryId", exception.ParamName);
        }
    }
}
