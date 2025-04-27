namespace CarRentalAPI.Models
{
    public class Rental : BaseEntity
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public double TotalPrice { get; set; }
        public string Status { get; set; } // Reserved, Active, Completed, Cancelled
        public string PickupLocation { get; set; }
        public string ReturnLocation { get; set; }
        public int CarId { get; set; }
        public string UserId { get; set; }

        // Navigation properties
        public virtual Car Car { get; set; }
        public virtual AppUser User { get; set; }
        public virtual Payment Payment { get; set; }
    }
}