using CarRentalAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CarRentalAPI.Repositories
{
    public class CarRepository : GenericRepository<Car>
    {
        public CarRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Car>> GetAllWithCategoryAsync()
        {
            return await _dbSet
                .Include(c => c.Category)
                .Where(c => c.IsActive)
                .ToListAsync();
        }

        public async Task<Car> GetByIdWithCategoryAsync(int id)
        {
            return await _dbSet
                .Include(c => c.Category)
                .FirstOrDefaultAsync(c => c.Id == id && c.IsActive);
        }

        public async Task<IEnumerable<Car>> GetAvailableCarsAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Include(c => c.Category)
                .Include(c => c.Rentals)
                .Where(c => c.IsActive && c.IsAvailable)
                .Where(c => !c.Rentals.Any(r =>
                    r.IsActive &&
                    ((startDate >= r.StartDate && startDate <= r.EndDate) ||
                     (endDate >= r.StartDate && endDate <= r.EndDate) ||
                     (startDate <= r.StartDate && endDate >= r.EndDate))))
                .ToListAsync();
        }

        public async Task<IEnumerable<Car>> GetCarsByCategoryAsync(int categoryId)
        {
            return await _dbSet
                .Include(c => c.Category)
                .Where(c => c.IsActive && c.CategoryId == categoryId)
                .ToListAsync();
        }
    }
}