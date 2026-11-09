using StockFlow.Domain.Entities;
using StockFlow.Domain.ValueObjects;

namespace StockFlow.Application.Common.Interfaces
{
    public interface IProductRepository
    {
        Task<bool> SkuExistsAsync(SkuValue sku, CancellationToken cancellationToken);
        Task AddAsync(Product product, CancellationToken cancellationToken);
    }
}