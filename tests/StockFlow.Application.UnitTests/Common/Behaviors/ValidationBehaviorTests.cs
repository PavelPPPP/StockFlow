using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using StockFlow.Application.Categories.Commands.DeleteCategory;
using StockFlow.Application.Common.Behaviors;
using StockFlow.Application.Common.Interfaces;
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

        [Fact]
        public async Task Send_NonGenericRequest_RunsValidationBehavior()
        {
            var repository = Substitute.For<ICategoryRepository>();
            repository.ExistsAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(false);

            var services = new ServiceCollection();
            services.AddSingleton<ILoggerFactory, NullLoggerFactory>();
            services.AddApplication();
            services.AddSingleton(repository);
            services.AddSingleton(Substitute.For<IUnitOfWork>());

            using var provider = services.BuildServiceProvider();
            var mediator = provider.GetRequiredService<IMediator>();

            await Assert.ThrowsAsync<ValidationException>(() =>
                mediator.Send(new DeleteCategoryCommand(Guid.NewGuid())));
        }
    }
}