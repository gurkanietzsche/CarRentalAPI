namespace CarRentalAPI.DTOs
{
    public class PaymentDTO
    {
        public int Id { get; set; }
        public double Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentMethod { get; set; }
        public string Status { get; set; }
        public string TransactionId { get; set; }
        public int RentalId { get; set; }
    }

    public class PaymentCreateDTO
    {
        public double Amount { get; set; }
        public string PaymentMethod { get; set; }
        public int RentalId { get; set; }
    }

    public class PaymentUpdateDTO
    {
        public int Id { get; set; }
        public string Status { get; set; }
        public string TransactionId { get; set; }
    }
}