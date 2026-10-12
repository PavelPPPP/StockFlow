using StockFlow.Domain.Entities;

namespace StockFlow.Application.Common.Interfaces
{
    public interface ICategoryRepository
    {
        Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken);
        Task<bool> HasChildrenAsync(Guid parentId, CancellationToken cancellationToken);
        Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task AddAsync(Category category, CancellationToken cancellationToken);
        void Remove(Category category);
    }
}
