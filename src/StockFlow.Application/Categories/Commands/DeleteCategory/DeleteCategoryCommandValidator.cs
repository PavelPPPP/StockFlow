using FluentValidation;
using StockFlow.Application.Common.Interfaces;

namespace StockFlow.Application.Categories.Commands.DeleteCategory
{
    public class DeleteCategoryCommandValidator : AbstractValidator<DeleteCategoryCommand>
    {
        private readonly ICategoryRepository _categoryRepository;

        public DeleteCategoryCommandValidator(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;

            RuleFor(x => x.Id)
                .MustAsync(Exist)
                .WithMessage("Category does not exist.");

            RuleFor(x => x.Id)
                .MustAsync(HaveNoChildren)
                .WithMessage("Category has child categories and cannot be deleted.")
                .When(x => x.Id != Guid.Empty);
        }

        private async Task<bool> Exist(Guid id, CancellationToken cancellationToken)
        {
            return await _categoryRepository.ExistsAsync(id, cancellationToken);
        }

        private async Task<bool> HaveNoChildren(Guid id, CancellationToken cancellationToken)
        {
            return !await _categoryRepository.HasChildrenAsync(id, cancellationToken);
        }
    }
}