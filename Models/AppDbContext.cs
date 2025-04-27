using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CarRentalAPI.Models
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Car> Cars { get; set; }
        public DbSet<CarCategory> CarCategories { get; set; }
        public DbSet<Rental> Rentals { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Review> Reviews { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Car ve Category ilişkisi
            modelBuilder.Entity<Car>()
                .HasOne(c => c.Category)
                .WithMany(cc => cc.Cars)
                .HasForeignKey(c => c.CategoryId);

            // Rental ve Car ilişkisi
            modelBuilder.Entity<Rental>()
                .HasOne(r => r.Car)
                .WithMany(c => c.Rentals)
                .HasForeignKey(r => r.CarId);

            // Rental ve User ilişkisi
            modelBuilder.Entity<Rental>()
                .HasOne(r => r.User)
                .WithMany(u => u.Rentals)
                .HasForeignKey(r => r.UserId);

            // Payment ve Rental ilişkisi
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Rental)
                .WithOne(r => r.Payment)
                .HasForeignKey<Payment>(p => p.RentalId);

            // Review ve User ilişkisi
            modelBuilder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserId);

            // Review ve Car ilişkisi
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Car)
                .WithMany(c => c.Reviews)
                .HasForeignKey(r => r.CarId);
        }
    }
}