using FluentValidation;
using StockFlow.Application.Common.Interfaces;

namespace StockFlow.Application.Products.Commands.CreateProduct
{
    public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
    {
        public CreateProductCommandValidator(
            IProductRepository productRepository,
            ICategoryRepository categoryRepository)
        {
            RuleFor(x => x.Name)
                .NotEmpty();

            RuleFor(x => x.Sku)
                .NotEmpty()
                .MaximumLength(30);
        }
    }
}