namespace CarRentalAPI.DTOs
{
    public class RentalDTO
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public double TotalPrice { get; set; }
        public string Status { get; set; }
        public string PickupLocation { get; set; }
        public string ReturnLocation { get; set; }
        public int CarId { get; set; }
        public string UserId { get; set; }
        public string CarBrand { get; set; }
        public string CarModel { get; set; }
        public string UserName { get; set; }
    }

    public class RentalCreateDTO
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string PickupLocation { get; set; }
        public string ReturnLocation { get; set; }
        public int CarId { get; set; }
    }

    public class RentalUpdateDTO
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; }
        public string PickupLocation { get; set; }
        public string ReturnLocation { get; set; }
    }
}