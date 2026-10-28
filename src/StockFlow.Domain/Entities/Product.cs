using StockFlow.Domain.Enums;
using StockFlow.Domain.ValueObjects;

namespace StockFlow.Domain.Entities
{
    public sealed class Product
    {
        public Guid Id { get; }
        public SkuValue Sku { get; }
        public string Name { get; private set; }
        public string? Description { get; private set; }
        public Guid CategoryId { get; private set; }
        public UnitOfMeasure UnitOfMeasure { get; private set; }
        public string? Barcode { get; private set; }

        private Product() { }
        private Product(
            Guid id,
            SkuValue sku,
            string name,
            string? description,
            Guid categoryId,
            UnitOfMeasure unitOfMeasure,
            string? barcode)
        {
            Id = id;
            Sku = sku;
            Name = name;
            Description = description;
            CategoryId = categoryId;
            UnitOfMeasure = unitOfMeasure;
            Barcode = barcode;
        }

        public static Product Create(
            SkuValue sku,
            string name,
            string? description,
            Guid categoryId,
            UnitOfMeasure unitOfMeasure,
            string? barcode)
        {
            EnsureNameNotEmpty(name);
            EnsureCategoryIdNotEmpty(categoryId);

            return new Product(
                Guid.NewGuid(),
                sku,
                name.Trim(),
                description,
                categoryId,
                unitOfMeasure,
                barcode);
        }

        public void UpdateDetails(
            string name,
            string? description,
            Guid categoryId,
            UnitOfMeasure unitOfMeasure,
            string? barcode)
        {
            EnsureNameNotEmpty(name);
            EnsureCategoryIdNotEmpty(categoryId);

            Name = name; 
            Description = description; 
            CategoryId = categoryId; 
            UnitOfMeasure = unitOfMeasure; 
            Barcode = barcode;
        }

        private static void EnsureNameNotEmpty(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Product name cannot be null or empty.", nameof(name));
            }
        }

        private static void EnsureCategoryIdNotEmpty(Guid categoryId)
        {
            if (categoryId == Guid.Empty)
            {
                throw new ArgumentException("CategoryId cannot be empty.", nameof(categoryId));
            }
        }
    }
}
