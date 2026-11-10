using NSubstitute;
using StockFlow.Application.Common.Interfaces;
using StockFlow.Application.Products.Commands.CreateProduct;
using StockFlow.Domain.Entities;
using StockFlow.Domain.Enums;
using Xunit;

namespace StockFlow.Application.UnitTests.Products.Commands.CreateProduct
{
    public class CreateProductCommandHandlerTests
    {
        [Fact]
        public async Task Handle_ValidCommand_AddsProductSavesAndReturnsId()
        {
            var categoryId = Guid.NewGuid();
            var productRepository = Substitute.For<IProductRepository>();
            var unitOfWork = Substitute.For<IUnitOfWork>();

            Product? addedProduct = null;
            await productRepository.AddAsync(
                Arg.Do<Product>(p => addedProduct = p),
                Arg.Any<CancellationToken>());

            var handler = new CreateProductCommandHandler(productRepository, unitOfWork);
            var command = new CreateProductCommand(
                Sku: "SKU-001",
                Name: "Laptop",
                Description: "Gaming laptop",
                CategoryId: categoryId,
                UnitOfMeasure: UnitOfMeasure.Pcs,
                Barcode: "1234567890");

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.NotNull(addedProduct);
            Assert.Equal(addedProduct.Id, result);
            Assert.Equal("SKU-001", addedProduct.Sku.Value);
            Assert.Equal("Laptop", addedProduct.Name);
            Assert.Equal(categoryId, addedProduct.CategoryId);
            await unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }
    }
}