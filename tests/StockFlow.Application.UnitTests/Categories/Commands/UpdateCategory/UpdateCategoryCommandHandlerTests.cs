using NSubstitute;
using StockFlow.Application.Categories.Commands.UpdateCategory;
using StockFlow.Application.Common.Interfaces;
using StockFlow.Domain.Entities;

namespace StockFlow.Application.UnitTests.Categories.Commands.UpdateCategory
{
    public class UpdateCategoryCommandHandlerTests
    {
        [Fact]
        public async Task Handle_ValidCommand_UpdateCategoryAndSaves()
        {
            var category = Category.Create("Old name", null);
            var repository = Substitute.For<ICategoryRepository>();
            repository.GetByIdAsync(category.Id, Arg.Any<CancellationToken>()).Returns(category);
            var unitOfWork = Substitute.For<IUnitOfWork>();

            var handler = new UpdateCategoryCommandHandler(repository, unitOfWork);
            var command = new UpdateCategoryCommand(category.Id, "New Name", null);

            await handler.Handle(command, CancellationToken.None);

            Assert.Equal("New Name", category.Name);
            await unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }
    }
}