using StockFlow.Domain.Common;

namespace StockFlow.Domain.Entities
{
    public class Category : Entity<Guid>
    {
        public string Name { get; private set; }
        public Guid? ParentCategoryId { get; private set; }

        private Category(Guid id, string name, Guid? parentCategoryId)
            : base(id)
        {
            Name = name;
            ParentCategoryId = parentCategoryId;
        }

        public static Category Create(string name, Guid? parentCategoryId)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Category name cannot be empty", nameof(name));
            }

            return new Category(Guid.NewGuid(), name, parentCategoryId);
        }
    }
}