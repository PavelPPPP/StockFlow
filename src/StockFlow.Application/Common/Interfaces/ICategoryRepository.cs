namespace StockFlow.Application.Common.Interfaces
{
    public interface ICategoryRepository
    {
        Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken);
    }
}
