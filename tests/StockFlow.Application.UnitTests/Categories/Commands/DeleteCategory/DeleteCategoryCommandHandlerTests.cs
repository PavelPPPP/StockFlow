using NSubstitute;
using StockFlow.Application.Categories.Commands.DeleteCategory;
using StockFlow.Application.Common.Interfaces;
using StockFlow.Domain.Entities;

namespace StockFlow.Application.UnitTests.Categories.Commands.DeleteCategory
{
    public class DeleteCategoryCommandHandlerTests
    {
        [Fact]
        public async Task Handle_ValidCommand_RemovesCategoryAndSaves()
        {
            var category = Category.Create("Electronics", null);
            var repository = Substitute.For<ICategoryRepository>();
            repository.GetByIdAsync(category.Id, Arg.Any<CancellationToken>()).Returns(category);
            var unitOfWork = Substitute.For<IUnitOfWork>();

            var handler = new DeleteCategoryCommandHandler(repository, unitOfWork);
            var command = new DeleteCategoryCommand(category.Id);

            await handler.Handle(command, CancellationToken.None);

            repository.Received(1).Remove(category);
            await unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }
    }
}