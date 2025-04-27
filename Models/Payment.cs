namespace CarRentalAPI.Models
{
    public class Payment : BaseEntity
    {
        public double Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentMethod { get; set; }
        public string Status { get; set; } // Pending, Completed, Failed
        public string TransactionId { get; set; }
        public int RentalId { get; set; }

        // Navigation property
        public virtual Rental Rental { get; set; }
    }
}