using StockFlow.Domain.Entities;
using NSubstitute;
using StockFlow.Application.Common.Interfaces;
using StockFlow.Application.Categories.Queries.GetCategoryById;

namespace StockFlow.Application.UnitTests.Categories.Queries.GetCategoryById
{
    public class GetCategoryByIdQueryHandlerTests
    {
        [Fact]
        public async Task Handle_ExistingId_ReturnsCategoryDto()
        {
            var category = Category.Create("Electronics", null);
            var repository = Substitute.For<ICategoryRepository>();
            repository.GetByIdAsync(category.Id, Arg.Any<CancellationToken>()).Returns(category);

            var handler = new GetCategoryByIdQueryHandler(repository);
            var query = new GetCategoryByIdQuery(category.Id);

            var result = await handler.Handle(query, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(category.Id, result!.Id);
            Assert.Equal(category.Name, result.Name);
        }
    }
}