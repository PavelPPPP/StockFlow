using MediatR;

namespace StockFlow.Application.Categories.Commands.UpdateCategory
{
    public record UpdateCategoryCommand(Guid Id, string Name, Guid? ParentCategoryId) : IRequest;
}