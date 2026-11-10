using FluentValidation;
using StockFlow.Application.Common.Interfaces;

namespace StockFlow.Application.Products.Commands.CreateProduct
{
    public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;

        public CreateProductCommandValidator(
            IProductRepository productRepository,
            ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;

            RuleFor(x => x.Name)
                .NotEmpty();

            RuleFor(x => x.Sku)
                .NotEmpty()
                .MaximumLength(30);

            RuleFor(x => x.CategoryId)
                .MustAsync(CategoryExists)
                .WithMessage("Category does not exist");
        }

        private async Task<bool> CategoryExists(Guid categoryId, CancellationToken cancellationToken)
        {
            return await _categoryRepository.ExistsAsync(categoryId, cancellationToken);
        }
    }
}