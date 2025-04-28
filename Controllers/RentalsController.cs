using System.Security.Claims;
using AutoMapper;
using CarRentalAPI.DTOs;
using CarRentalAPI.Models;
using CarRentalAPI.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarRentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RentalsController : ControllerBase
    {
        private readonly RentalRepository _rentalRepository;
        private readonly CarRepository _carRepository;
        private readonly IMapper _mapper;

        public RentalsController(RentalRepository rentalRepository, CarRepository carRepository, IMapper mapper)
        {
            _rentalRepository = rentalRepository;
            _carRepository = carRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var rentals = await _rentalRepository.GetAllWithDetailsAsync();
            var rentalDtos = _mapper.Map<IEnumerable<RentalDTO>>(rentals);
            return Ok(rentalDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var rental = await _rentalRepository.GetByIdWithDetailsAsync(id);
            if (rental == null)
                return NotFound();

            // Check if the current user is the owner or an admin
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (rental.UserId != userId && !User.IsInRole("Admin"))
                return Forbid();

            var rentalDto = _mapper.Map<RentalDTO>(rental);
            return Ok(rentalDto);
        }

        [HttpGet("my-rentals")]
        public async Task<IActionResult> GetMyRentals()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var rentals = await _rentalRepository.GetUserRentalsAsync(userId);
            var rentalDtos = _mapper.Map<IEnumerable<RentalDTO>>(rentals);
            return Ok(rentalDtos);
        }

        [HttpGet("car/{carId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetCarRentals(int carId)
        {
            var rentals = await _rentalRepository.GetCarRentalsAsync(carId);
            var rentalDtos = _mapper.Map<IEnumerable<RentalDTO>>(rentals);
            return Ok(rentalDtos);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RentalCreateDTO rentalDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Check if car is available for the requested dates
            var availableCars = await _carRepository.GetAvailableCarsAsync(rentalDto.StartDate, rentalDto.EndDate);
            if (!availableCars.Any(c => c.Id == rentalDto.CarId))
                return BadRequest(new { Message = "Car is not available for the selected dates" });

            var car = await _carRepository.GetByIdAsync(rentalDto.CarId);

            // Calculate total price
            var days = (rentalDto.EndDate - rentalDto.StartDate).Days + 1;
            var totalPrice = days * car.DailyRate;

            var rental = _mapper.Map<Rental>(rentalDto);
            rental.UserId = userId;
            rental.TotalPrice = totalPrice;
            rental.Status = "Reserved";

            await _rentalRepository.AddAsync(rental);
            var rentalResultDto = _mapper.Map<RentalDTO>(rental);

            return CreatedAtAction(nameof(GetById), new { id = rental.Id }, rentalResultDto);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] RentalUpdateDTO rentalDto)
        {
            var existingRental = await _rentalRepository.GetByIdAsync(rentalDto.Id);
            if (existingRental == null)
                return NotFound();

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (existingRental.UserId != userId && !User.IsInRole("Admin"))
                return Forbid();

            // If dates are changed, check availability
            if (existingRental.StartDate != rentalDto.StartDate || existingRental.EndDate != rentalDto.EndDate)
            {
                var availableCars = await _carRepository.GetAvailableCarsAsync(rentalDto.StartDate, rentalDto.EndDate);
                if (!availableCars.Any(c => c.Id == existingRental.CarId))
                    return BadRequest(new { Message = "Car is not available for the selected dates" });

                // Recalculate total price
                var car = await _carRepository.GetByIdAsync(existingRental.CarId);
                var days = (rentalDto.EndDate - rentalDto.StartDate).Days + 1;
                existingRental.TotalPrice = days * car.DailyRate;
            }

            _mapper.Map(rentalDto, existingRental);
            await _rentalRepository.UpdateAsync(existingRental);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var rental = await _rentalRepository.GetByIdAsync(id);
            if (rental == null)
                return NotFound();

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (rental.UserId != userId && !User.IsInRole("Admin"))
                return Forbid();

            var result = await _rentalRepository.DeleteAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}