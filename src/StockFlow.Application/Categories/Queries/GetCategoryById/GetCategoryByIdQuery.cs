using MediatR;

namespace StockFlow.Application.Categories.Queries.GetCategoryById
{
    public record GetCategoryByIdQuery(Guid Id) : IRequest<CategoryDto?>;
}