namespace CarRentalAPI.DTOs
{
    public class ReviewDTO
    {
        public int Id { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public int CarId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string CarBrand { get; set; }
        public string CarModel { get; set; }
    }

    public class ReviewCreateDTO
    {
        public int Rating { get; set; }
        public string Comment { get; set; }
        public int CarId { get; set; }
    }

    public class ReviewUpdateDTO
    {
        public int Id { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
    }
}