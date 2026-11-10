using FluentValidation;
using StockFlow.Application.Common.Interfaces;
using StockFlow.Domain.ValueObjects;

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
                .MaximumLength(30)
                .MustAsync(SkuIsUnique)
                .WithMessage("SKU already exists");

            RuleFor(x => x.CategoryId)
                .MustAsync(CategoryExists)
                .WithMessage("Category does not exist");
        }

        private async Task<bool> SkuIsUnique(string sku, CancellationToken cancellationToken)
        {
            SkuValue skuValue;
            try
            {
                skuValue = SkuValue.Create(sku);
            }
            catch (ArgumentException)
            {
                // The format checks the domain: an error will be returned as an ArgumentException → 400.
                return true;
            }

            return !await _productRepository.SkuExistsAsync(skuValue, cancellationToken);
        }

        private async Task<bool> CategoryExists(Guid categoryId, CancellationToken cancellationToken)
        {
            return await _categoryRepository.ExistsAsync(categoryId, cancellationToken);
        }
    }
}