using NSubstitute;
using StockFlow.Application.Common.Interfaces;
using StockFlow.Application.Products.Commands.CreateProduct;
using StockFlow.Domain.Enums;
using StockFlow.Domain.ValueObjects;
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

        [Fact]
        public async Task Validate_EmptySku_ReturnsError()
        {
            var productRepository = Substitute.For<IProductRepository>();
            var categoryRepository = Substitute.For<ICategoryRepository>();
            var validator = new CreateProductCommandValidator(productRepository, categoryRepository);
            var command = new CreateProductCommand(
                Sku: "",
                Name: "Laptop",
                Description: null,
                CategoryId: Guid.NewGuid(),
                UnitOfMeasure: UnitOfMeasure.Pcs,
                Barcode: null);

            var result = await validator.ValidateAsync(command);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.Sku));
        }

        [Fact]
        public async Task Validate_SkuLongerThan30Characters_ReturnsError()
        {
            var productRepository = Substitute.For<IProductRepository>();
            var categoryRepository = Substitute.For<ICategoryRepository>();
            var validator = new CreateProductCommandValidator(productRepository, categoryRepository);
            var command = new CreateProductCommand(
                Sku: new string('A', 31),
                Name: "Laptop",
                Description: null,
                CategoryId: Guid.NewGuid(),
                UnitOfMeasure: UnitOfMeasure.Pcs,
                Barcode: null);

            var result = await validator.ValidateAsync(command);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.Sku));
        }

        [Fact]
        public async Task Validate_CategoryDoesNotExist_ReturnsError()
        {
            var categoryId = Guid.NewGuid();
            var productRepository = Substitute.For<IProductRepository>();
            var categoryRepository = Substitute.For<ICategoryRepository>();
            categoryRepository.ExistsAsync(categoryId, Arg.Any<CancellationToken>()).Returns(false);

            var validator = new CreateProductCommandValidator(productRepository, categoryRepository);
            var command = new CreateProductCommand(
                Sku: "SKU-001",
                Name: "Laptop",
                Description: null,
                CategoryId: categoryId,
                UnitOfMeasure: UnitOfMeasure.Pcs,
                Barcode: null);

            var result = await validator.ValidateAsync(command);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.CategoryId));
        }

        [Fact]
        public async Task Validate_SkuAlreadyExists_ReturnsError()
        {
            var categoryId = Guid.NewGuid();
            var productRepository = Substitute.For<IProductRepository>();
            productRepository.SkuExistsAsync(SkuValue.Create("SKU-001"), Arg.Any<CancellationToken>()).Returns(true);
            var categoryRepository = Substitute.For<ICategoryRepository>();
            categoryRepository.ExistsAsync(categoryId, Arg.Any<CancellationToken>()).Returns(true);

            var validator = new CreateProductCommandValidator(productRepository, categoryRepository);
            var command = new CreateProductCommand(
                Sku: "SKU-001",
                Name: "Laptop",
                Description: null,
                CategoryId: categoryId,
                UnitOfMeasure: UnitOfMeasure.Pcs,
                Barcode: null);

            var result = await validator.ValidateAsync(command);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.Sku));
        }

        [Fact]
        public async Task Validate_ValidCommand_ReturnsNoError()
        {
            var categoryId = Guid.NewGuid();
            var productRepository = Substitute.For<IProductRepository>();
            productRepository.SkuExistsAsync(SkuValue.Create("SKU-001"), Arg.Any<CancellationToken>()).Returns(false);
            var categoryRepository = Substitute.For<ICategoryRepository>();
            categoryRepository.ExistsAsync(categoryId, Arg.Any<CancellationToken>()).Returns(true);

            var validator = new CreateProductCommandValidator(productRepository, categoryRepository);
            var command = new CreateProductCommand(
                Sku: "SKU-001",
                Name: "Laptop",
                Description: null,
                CategoryId: categoryId,
                UnitOfMeasure: UnitOfMeasure.Pcs,
                Barcode: null);

            var result = await validator.ValidateAsync(command);

            Assert.True(result.IsValid);
        }
    }
}