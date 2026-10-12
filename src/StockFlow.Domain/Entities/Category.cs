using StockFlow.Domain.Common;
using StockFlow.Domain.Exceptions;

namespace StockFlow.Domain.Entities
{
    public class Category : Entity<Guid>
    {
        public string Name { get; private set; }
        public Guid? ParentCategoryId { get; private set; }

        private Category() { }
        private Category(Guid id, string name, Guid? parentCategoryId)
            : base(id)
        {
            Name = name;
            ParentCategoryId = parentCategoryId;
        }

        public static Category Create(string name, Guid? parentCategoryId)
        {
            EnsureNotEmpty(name);

            return new Category(Guid.NewGuid(), name.Trim(), parentCategoryId);
        }

        public void UpdateDetails(string name, Guid? parentCategoryId)
        {
            EnsureNotEmpty(name);

            if (parentCategoryId == Id)
            {
                throw new CategorySelfParentException(Id);
            }

            Name = name.Trim();
            ParentCategoryId = parentCategoryId;
        }

        private static void EnsureNotEmpty(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Category name cannot be empty", nameof(name));
            }
        }
    }
}