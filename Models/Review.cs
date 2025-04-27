namespace CarRentalAPI.Models
{
    public class Review : BaseEntity
    {
        public int Rating { get; set; } // 1-5
        public string Comment { get; set; }
        public int CarId { get; set; }
        public string UserId { get; set; }

        // Navigation properties
        public virtual Car Car { get; set; }
        public virtual AppUser User { get; set; }
    }
}