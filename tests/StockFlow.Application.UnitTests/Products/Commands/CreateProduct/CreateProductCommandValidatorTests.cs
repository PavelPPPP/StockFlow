using NSubstitute;
using StockFlow.Application.Common.Interfaces;
using StockFlow.Application.Products.Commands.CreateProduct;
using StockFlow.Domain.Enums;
using Xunit;

namespace StockFlow.Application.UnitTests.Products.Commands.CreateProduct
{
    public class CreateProductCommandValidatorTests
    {
        [Fact]
        public async Task Validate_EmptyName_ReturnsError()
        {
            var productRepository = Substitute.For<IProductRepository>();
            var categoryRepository = Substitute.For<ICategoryRepository>();
            var validator = new CreateProductCommandValidator(productRepository, categoryRepository);
            var command = new CreateProductCommand(
                Sku: "SKU-001",
                Name: "",
                Description: null,
                CategoryId: Guid.NewGuid(),
                UnitOfMeasure: UnitOfMeasure.Pcs,
                Barcode: null);

            var result = await validator.ValidateAsync(command);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.Name));
        }
    }
}