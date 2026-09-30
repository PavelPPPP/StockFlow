using NSubstitute;
using StockFlow.Application.Categories.Commands.CreateCategory;
using StockFlow.Application.Common.Interfaces;
using Xunit;

namespace StockFlow.Application.UnitTests.Categories.Commands.CreateCategory
{
    public class CreateCategoryCommandValidatorTests
    {
        [Fact]
        public async Task Validate_EmptyName_ReturnsError()
        {
            var repository = Substitute.For<ICategoryRepository>();
            var validator = new CreateCategoryCommandValidator(repository);
            var command = new CreateCategoryCommand(Name: "", ParentCategoryId: null);

            var result = await validator.ValidateAsync(command);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.Name));
        }

        [Fact]
        public async Task Validate_ParentCategoryIdDoesNotExist_ReturnError()
        {
            var parentId = Guid.NewGuid();
            var repository = Substitute.For<ICategoryRepository>();
            repository.ExistsAsync(parentId, Arg.Any<CancellationToken>()).Returns(false);

            var validator = new CreateCategoryCommandValidator(repository);
            var command = new CreateCategoryCommand(Name: "Electronics", ParentCategoryId: parentId);

            var result = await validator.ValidateAsync(command);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.ParentCategoryId));
        }
    }
}