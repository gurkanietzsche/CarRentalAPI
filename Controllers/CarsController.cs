using CarRentalAPI.DTOs;
using CarRentalAPI.Models;
using CarRentalAPI.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarRentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarsController : ControllerBase
    {
        private readonly CarRepository _carRepository;

        public CarsController(CarRepository carRepository)
        {
            _carRepository = carRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var cars = await _carRepository.GetAllWithCategoryAsync();
            var carDtos = cars.Select(c => new CarDTO
            {
                Id = c.Id,
                Brand = c.Brand,
                Model = c.Model,
                Year = c.Year,
                Color = c.Color,
                LicensePlate = c.LicensePlate,
                DailyRate = c.DailyRate,
                IsAvailable = c.IsAvailable,
                ImageUrl = c.ImageUrl,
                Description = c.Description,
                Mileage = c.Mileage,
                FuelType = c.FuelType,
                Transmission = c.Transmission,
                Seats = c.Seats,
                HasAC = c.HasAC,
                HasGPS = c.HasGPS,
                CategoryId = c.CategoryId,
                CategoryName = c.Category?.Name
            });

            return Ok(carDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var car = await _carRepository.GetByIdWithCategoryAsync(id);
            if (car == null)
                return NotFound();

            var carDto = new CarDTO
            {
                Id = car.Id,
                Brand = car.Brand,
                Model = car.Model,
                Year = car.Year,
                Color = car.Color,
                LicensePlate = car.LicensePlate,
                DailyRate = car.DailyRate,
                IsAvailable = car.IsAvailable,
                ImageUrl = car.ImageUrl,
                Description = car.Description,
                Mileage = car.Mileage,
                FuelType = car.FuelType,
                Transmission = car.Transmission,
                Seats = car.Seats,
                HasAC = car.HasAC,
                HasGPS = car.HasGPS,
                CategoryId = car.CategoryId,
                CategoryName = car.Category?.Name
            };

            return Ok(carDto);
        }

        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableCars([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var cars = await _carRepository.GetAvailableCarsAsync(startDate, endDate);
            var carDtos = cars.Select(c => new CarDTO
            {
                Id = c.Id,
                Brand = c.Brand,
                Model = c.Model,
                Year = c.Year,
                Color = c.Color,
                LicensePlate = c.LicensePlate,
                DailyRate = c.DailyRate,
                IsAvailable = c.IsAvailable,
                ImageUrl = c.ImageUrl,
                Description = c.Description,
                Mileage = c.Mileage,
                FuelType = c.FuelType,
                Transmission = c.Transmission,
                Seats = c.Seats,
                HasAC = c.HasAC,
                HasGPS = c.HasGPS,
                CategoryId = c.CategoryId,
                CategoryName = c.Category?.Name
            });

            return Ok(carDtos);
        }

        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> GetCarsByCategory(int categoryId)
        {
            var cars = await _carRepository.GetCarsByCategoryAsync(categoryId);
            var carDtos = cars.Select(c => new CarDTO
            {
                Id = c.Id,
                Brand = c.Brand,
                Model = c.Model,
                Year = c.Year,
                Color = c.Color,
                LicensePlate = c.LicensePlate,
                DailyRate = c.DailyRate,
                IsAvailable = c.IsAvailable,
                ImageUrl = c.ImageUrl,
                Description = c.Description,
                Mileage = c.Mileage,
                FuelType = c.FuelType,
                Transmission = c.Transmission,
                Seats = c.Seats,
                HasAC = c.HasAC,
                HasGPS = c.HasGPS,
                CategoryId = c.CategoryId,
                CategoryName = c.Category?.Name
            });

            return Ok(carDtos);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CarCreateDTO carDto)
        {
            var car = new Car
            {
                Brand = carDto.Brand,
                Model = carDto.Model,
                Year = carDto.Year,
                Color = carDto.Color,
                LicensePlate = carDto.LicensePlate,
                DailyRate = carDto.DailyRate,
                IsAvailable = true,
                ImageUrl = carDto.ImageUrl,
                Description = carDto.Description,
                Mileage = carDto.Mileage,
                FuelType = carDto.FuelType,
                Transmission = carDto.Transmission,
                Seats = carDto.Seats,
                HasAC = carDto.HasAC,
                HasGPS = carDto.HasGPS,
                CategoryId = carDto.CategoryId
            };

            await _carRepository.AddAsync(car);
            return CreatedAtAction(nameof(GetById), new { id = car.Id }, car);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] CarUpdateDTO carDto)
        {
            var existingCar = await _carRepository.GetByIdAsync(carDto.Id);
            if (existingCar == null)
                return NotFound();

            existingCar.Brand = carDto.Brand;
            existingCar.Model = carDto.Model;
            existingCar.Year = carDto.Year;
            existingCar.Color = carDto.Color;
            existingCar.LicensePlate = carDto.LicensePlate;
            existingCar.DailyRate = carDto.DailyRate;
            existingCar.IsAvailable = carDto.IsAvailable;
            existingCar.ImageUrl = carDto.ImageUrl;
            existingCar.Description = carDto.Description;
            existingCar.Mileage = carDto.Mileage;
            existingCar.FuelType = carDto.FuelType;
            existingCar.Transmission = carDto.Transmission;
            existingCar.Seats = carDto.Seats;
            existingCar.HasAC = carDto.HasAC;
            existingCar.HasGPS = carDto.HasGPS;
            existingCar.CategoryId = carDto.CategoryId;

            await _carRepository.UpdateAsync(existingCar);
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _carRepository.DeleteAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}