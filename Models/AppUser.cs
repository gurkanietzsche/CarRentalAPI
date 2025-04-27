using CarRentalAPI.Models;
using Microsoft.AspNetCore.Identity;

public class AppUser : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? Address { get; set; } // Nullable yapın
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? PostalCode { get; set; }
    public string? DriverLicenseNumber { get; set; }
    public DateTime? BirthDate { get; set; } // DateTime tipini de nullable yapın
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public DateTime? ModifiedDate { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public virtual ICollection<Rental> Rentals { get; set; }
    public virtual ICollection<Review> Reviews { get; set; }
}