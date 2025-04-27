using CarRentalAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CarRentalAPI.Repositories
{
    public class ReviewRepository : GenericRepository<Review>
    {
        public ReviewRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Review>> GetAllWithDetailsAsync()
        {
            return await _dbSet
                .Include(r => r.Car)
                .Include(r => r.User)
                .Where(r => r.IsActive)
                .ToListAsync();
        }

        public async Task<Review> GetByIdWithDetailsAsync(int id)
        {
            return await _dbSet
                .Include(r => r.Car)
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == id && r.IsActive);
        }

        public async Task<IEnumerable<Review>> GetCarReviewsAsync(int carId)
        {
            return await _dbSet
                .Include(r => r.User)
                .Where(r => r.IsActive && r.CarId == carId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Review>> GetUserReviewsAsync(string userId)
        {
            return await _dbSet
                .Include(r => r.Car)
                .Where(r => r.IsActive && r.UserId == userId)
                .ToListAsync();
        }
    }
}