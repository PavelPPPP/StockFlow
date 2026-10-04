using FluentValidation;
using FluentValidation.Results;
using MediatR;
using NSubstitute;
using StockFlow.Application.Common.Behaviors;
using ValidationException = StockFlow.Application.Common.Exceptions.ValidationException;

namespace StockFlow.Application.UnitTests.Common.Behaviors
{
    public record TestCommand(string Value) : IRequest<string>;

    public class ValidationBehaviorTests
    {
        [Fact]
        public async Task Handle_FailingValidator_ThrowsValidationException()
        {
            var validator = Substitute.For<IValidator<TestCommand>>();
            validator.ValidateAsync(Arg.Any<ValidationContext<TestCommand>>(), Arg.Any<CancellationToken>())
                .Returns(new ValidationResult(new[] { new ValidationFailure("Value", "Value is required") }));

            var behavior = new ValidationBehavior<TestCommand, string>(new[] { validator });
            var command = new TestCommand("");

            await Assert.ThrowsAsync<ValidationException>(() => 
                behavior.Handle(command, _ => Task.FromResult("result"), CancellationToken.None));
        }

        [Fact]
        public async Task Handle_PassingValidator_CallsNext()
        {
            var validator = Substitute.For<IValidator<TestCommand>>();
            validator.ValidateAsync(Arg.Any<ValidationContext<TestCommand>>(), Arg.Any<CancellationToken>())
                .Returns(new ValidationResult());

            var behavior = new ValidationBehavior<TestCommand, string>(new[] { validator });
            var command = new TestCommand("valid");

            var result = await behavior.Handle(command, _ => Task.FromResult("result"), CancellationToken.None);

            Assert.Equal("result", result);
        }
    }
}