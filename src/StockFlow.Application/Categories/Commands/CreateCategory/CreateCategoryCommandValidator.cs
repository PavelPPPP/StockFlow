using FluentValidation;
using StockFlow.Application.Common.Interfaces;

namespace StockFlow.Application.Categories.Commands.CreateCategory
{
    public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
    {
        private readonly ICategoryRepository _categoryRepository;

        public CreateCategoryCommandValidator(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;

            RuleFor(x => x.Name)
                .NotEmpty();

            RuleFor(x => x.ParentCategoryId)
                .MustAsync(ExistIfProvided)
                .WithMessage("Parent category does not exist")
                .When(x => x.ParentCategoryId.HasValue);
        }

        private async Task<bool> ExistIfProvided(Guid? parentCategoryId, CancellationToken cancellationToken)
        {
            return await _categoryRepository.ExistsAsync(parentCategoryId!.Value, cancellationToken);
        }
    }
}
