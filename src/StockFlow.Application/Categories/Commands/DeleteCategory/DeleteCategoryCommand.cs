using MediatR;

namespace StockFlow.Application.Categories.Commands.DeleteCategory
{
    public record DeleteCategoryCommand(Guid Id) : IRequest;
}