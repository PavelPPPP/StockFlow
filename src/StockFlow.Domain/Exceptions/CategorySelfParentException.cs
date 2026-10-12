namespace StockFlow.Domain.Exceptions
{
    public class CategorySelfParentException : DomainException
    {
        public CategorySelfParentException(Guid categoryId)
            : base($"Category '{categoryId}' cannot be its own parent.")
        {
        }
    }
}