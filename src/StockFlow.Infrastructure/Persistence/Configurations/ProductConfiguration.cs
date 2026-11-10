using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockFlow.Domain.Entities;
using StockFlow.Domain.ValueObjects;

namespace StockFlow.Infrastructure.Persistence.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Sku)
                .HasConversion(sku => sku.Value, value => SkuValue.Create(value))
                .IsRequired()
                .HasMaxLength(30);

            builder.HasIndex(p => p.Sku).IsUnique();

            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(p => p.Description)
                .HasMaxLength(1000);

            builder.Property(p => p.Barcode)
                .HasMaxLength(50);

            builder.Property(p => p.UnitOfMeasure)
                .HasConversion<string>()
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(p => p.IsActive)
                .IsRequired();

            builder.HasOne<Category>()
                .WithMany()
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}