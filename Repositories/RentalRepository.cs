using CarRentalAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CarRentalAPI.Repositories
{
    public class RentalRepository : GenericRepository<Rental>
    {
        public RentalRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Rental>> GetAllWithDetailsAsync()
        {
            return await _dbSet
                .Include(r => r.Car)
                .Include(r => r.User)
                .Include(r => r.Payment)
                .Where(r => r.IsActive)
                .ToListAsync();
        }

        public async Task<Rental> GetByIdWithDetailsAsync(int id)
        {
            return await _dbSet
                .Include(r => r.Car)
                .Include(r => r.User)
                .Include(r => r.Payment)
                .FirstOrDefaultAsync(r => r.Id == id && r.IsActive);
        }

        public async Task<IEnumerable<Rental>> GetUserRentalsAsync(string userId)
        {
            return await _dbSet
                .Include(r => r.Car)
                .Include(r => r.Payment)
                .Where(r => r.IsActive && r.UserId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Rental>> GetCarRentalsAsync(int carId)
        {
            return await _dbSet
                .Include(r => r.User)
                .Include(r => r.Payment)
                .Where(r => r.IsActive && r.CarId == carId)
                .ToListAsync();
        }
    }
}