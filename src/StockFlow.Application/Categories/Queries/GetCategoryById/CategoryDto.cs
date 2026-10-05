namespace StockFlow.Application.Categories.Queries.GetCategoryById
{
    public record CategoryDto(Guid Id, string Name, Guid? ParentCategory);
}