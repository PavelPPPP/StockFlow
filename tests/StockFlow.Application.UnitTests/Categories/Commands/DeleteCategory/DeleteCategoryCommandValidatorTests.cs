using NSubstitute;
using StockFlow.Application.Common.Interfaces;
using StockFlow.Application.Categories.Commands.DeleteCategory;

namespace StockFlow.Application.UnitTests.Categories.Commands.DeleteCategory
{
    public class DeleteCategoryCommandValidatorTests
    {
        [Fact]
        public async Task Validate_CategoryHasChildren_ResultsError()
        {
            var id = Guid.NewGuid();
            var repository = Substitute.For<ICategoryRepository>();
            repository.ExistsAsync(id, Arg.Any<CancellationToken>()).Returns(true);
            repository.HasChildrenAsync(id, Arg.Any<CancellationToken>()).Returns(true);

            var validator = new DeleteCategoryCommandValidator(repository);
            var command = new DeleteCategoryCommand(id);

            var result = await validator.ValidateAsync(command);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.Id));
        }
    }
}