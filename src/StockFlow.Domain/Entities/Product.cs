using StockFlow.Domain.Enums;
using StockFlow.Domain.ValueObjects;

namespace StockFlow.Domain.Entities
{
    public sealed class Product
    {
        public Guid Id { get; }
        public SkuValue Sku { get; }
        public string Name { get; }
        public string? Description { get; }
        public Guid CategoryId { get; }
        public UnitOfMeasure UnitOfMeasure { get; }
        public string? Barcode { get; }

        private Product(
            Guid id,
            SkuValue sku,
            string name,
            string? description,
            Guid categoryId,
            UnitOfMeasure unitOfMeasure,
            string? barcode)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Product name cannot be null or empty.", nameof(name));
            }

            if (categoryId == Guid.Empty)
            {
                throw new ArgumentException("CategoryId cannot be empty.", nameof(name));
            }

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
            return new Product(
                Guid.NewGuid(),
                sku,
                name,
                description,
                categoryId,
                unitOfMeasure,
                barcode);
        }
    }
}
