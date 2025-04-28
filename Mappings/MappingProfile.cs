using AutoMapper;
using CarRentalAPI.DTOs;
using CarRentalAPI.Models;

namespace CarRentalAPI.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Car Mappings
            CreateMap<Car, CarDTO>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name));

            CreateMap<CarCreateDTO, Car>();

            CreateMap<CarUpdateDTO, Car>();

            // Category Mappings
            CreateMap<CarCategory, CategoryDTO>();
            CreateMap<CategoryCreateDTO, CarCategory>();
            CreateMap<CategoryUpdateDTO, CarCategory>();

            // Rental Mappings
            CreateMap<Rental, RentalDTO>()
                .ForMember(dest => dest.CarBrand, opt => opt.MapFrom(src => src.Car.Brand))
                .ForMember(dest => dest.CarModel, opt => opt.MapFrom(src => src.Car.Model))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"));

            CreateMap<RentalCreateDTO, Rental>();
            CreateMap<RentalUpdateDTO, Rental>();

            // Payment Mappings
            CreateMap<Payment, PaymentDTO>();
            CreateMap<PaymentCreateDTO, Payment>();
            CreateMap<PaymentUpdateDTO, Payment>();

            // Review Mappings
            CreateMap<Review, ReviewDTO>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest => dest.CarBrand, opt => opt.MapFrom(src => src.Car.Brand))
                .ForMember(dest => dest.CarModel, opt => opt.MapFrom(src => src.Car.Model));

            CreateMap<ReviewCreateDTO, Review>();
            CreateMap<ReviewUpdateDTO, Review>();

            // User Mappings
            CreateMap<AppUser, UserDTO>()
                .ForMember(dest => dest.Roles, opt => opt.Ignore()); // Roles will be handled separately

            CreateMap<RegisterDTO, AppUser>();
        }
    }
}