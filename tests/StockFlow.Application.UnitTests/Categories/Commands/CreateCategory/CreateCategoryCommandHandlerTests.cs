using NSubstitute;
using StockFlow.Application.Categories.Commands.CreateCategory;
using StockFlow.Application.Common.Interfaces;
using StockFlow.Domain.Entities;

namespace StockFlow.Application.UnitTests.Categories.Commands.CreateCategory
{
    public class CreateCategoryCommandHandlerTests
    {
        [Fact]
        public async Task Handle_ValidCommand_ReturnsNewCategoryId()
        {
            var repository = Substitute.For<ICategoryRepository>();
            var unitOfWork = Substitute.For<IUnitOfWork>();
            var handler = new CreateCategoryCommandHandler(repository, unitOfWork);
            var command = new CreateCategoryCommand(Name: "Electrinix", ParentCategoryId: null);

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.NotEqual(Guid.Empty, result);
            await repository.Received(1).AddAsync(Arg.Any<Category>(), Arg.Any<CancellationToken>());
            await unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }
    }
}