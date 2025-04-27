namespace CarRentalAPI.Models
{
    public class CarCategory : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }

        // Navigation property
        public virtual ICollection<Car> Cars { get; set; }
    }
}