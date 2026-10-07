using NSubstitute;
using StockFlow.Application.Common.Interfaces;
using StockFlow.Application.Categories.Commands.UpdateCategory;

namespace StockFlow.Application.UnitTests.Categories.Commands.UpdateCategory
{
    public class UpdateCategoryCommandValidatorTests
    {
        [Fact]
        public async Task Validate_NonExistingId_ReturnError()
        {
            var repository = Substitute.For<ICategoryRepository>();
            repository.ExistsAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(false);

            var validator = new UpdateCategoryCommandValidator(repository);
            var command = new UpdateCategoryCommand(Guid.NewGuid(), "Electronics", null);

            var result = await validator.ValidateAsync(command);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.Id));
        }

        [Fact]
        public async Task Validate_EmptyName_ReturnErrors()
        {
            var repository = Substitute.For<ICategoryRepository>();
            repository.ExistsAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(true);

            var validator = new UpdateCategoryCommandValidator(repository);
            var command = new UpdateCategoryCommand(Guid.NewGuid(), "", null);

            var result = await validator.ValidateAsync(command);
            
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.Name));
        }

        [Fact]
        public async Task Validate_ValidCommandWithoutParent_ReturnsNoError()
        {
            var repository = Substitute.For<ICategoryRepository>();
            repository.ExistsAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(true);

            var validator = new UpdateCategoryCommandValidator(repository);
            var command = new UpdateCategoryCommand(Guid.NewGuid(), "Electronics", null);

            var result = await validator.ValidateAsync(command);

            Assert.True(result.IsValid);
        }
    }
}