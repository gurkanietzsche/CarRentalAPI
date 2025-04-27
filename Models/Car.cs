namespace CarRentalAPI.Models
{
    public class Car : BaseEntity
    {
        public string Brand { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public string Color { get; set; }
        public string LicensePlate { get; set; }
        public double DailyRate { get; set; }
        public bool IsAvailable { get; set; } = true;
        public string ImageUrl { get; set; }
        public string Description { get; set; }
        public int Mileage { get; set; }
        public string FuelType { get; set; }
        public string Transmission { get; set; }
        public int Seats { get; set; }
        public bool HasAC { get; set; }
        public bool HasGPS { get; set; }
        public int CategoryId { get; set; }

        // Navigation properties
        public virtual CarCategory Category { get; set; }
        public virtual ICollection<Rental> Rentals { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
    }
}