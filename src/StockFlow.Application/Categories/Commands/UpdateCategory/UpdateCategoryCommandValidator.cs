using FluentValidation;
using StockFlow.Application.Common.Interfaces;

namespace StockFlow.Application.Categories.Commands.UpdateCategory
{
    public class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
    {
        private readonly ICategoryRepository _categoryRepository;

        public UpdateCategoryCommandValidator(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;

            RuleFor(x => x.Id)
                .MustAsync(Exist)
                .WithMessage("Category does not exist.");

            RuleFor(x => x.Name)
                .NotEmpty();

            RuleFor(x => x.ParentCategoryId)
                .MustAsync(ExistIfProvided)
                .WithMessage("Parent category does not exist.")
                .When(x => x.ParentCategoryId.HasValue);
        }

        private async Task<bool> Exist(Guid id, CancellationToken cancellationToken)
        {
            return await _categoryRepository.ExistsAsync(id, cancellationToken);
        }

        private async Task<bool> ExistIfProvided(Guid? parentCategoryId, CancellationToken cancellationToken)
        {
            return await _categoryRepository.ExistsAsync(parentCategoryId!.Value, cancellationToken);
        }
    }
}