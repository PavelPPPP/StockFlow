using Microsoft.EntityFrameworkCore;
using StockFlow.Application.Common.Interfaces;
using StockFlow.Domain.Entities;

namespace StockFlow.Infrastructure.Persistence.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _context.Categories.AnyAsync(c => c.Id == id, cancellationToken);
        }

        public async Task AddAsync(Category category, CancellationToken cancellationToken)
        {
            await _context.Categories.AddAsync(category, cancellationToken);
        }
    }
}