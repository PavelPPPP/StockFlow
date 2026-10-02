using MediatR;

namespace StockFlow.Application.Categories.Commands.CreateCategory
{
    public record CreateCategoryCommand(string Name, Guid? ParentCategoryId) : IRequest<Guid>;
}