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
    public class CarsController : ControllerBase
    {
        private readonly CarRepository _carRepository;
        private readonly IMapper _mapper;

        public CarsController(CarRepository carRepository, IMapper mapper)
        {
            _carRepository = carRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var cars = await _carRepository.GetAllWithCategoryAsync();
            var carDtos = _mapper.Map<IEnumerable<CarDTO>>(cars);
            return Ok(carDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var car = await _carRepository.GetByIdWithCategoryAsync(id);
            if (car == null)
                return NotFound();

            var carDto = _mapper.Map<CarDTO>(car);
            return Ok(carDto);
        }

        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableCars([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var cars = await _carRepository.GetAvailableCarsAsync(startDate, endDate);
            var carDtos = _mapper.Map<IEnumerable<CarDTO>>(cars);
            return Ok(carDtos);
        }

        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> GetCarsByCategory(int categoryId)
        {
            var cars = await _carRepository.GetCarsByCategoryAsync(categoryId);
            var carDtos = _mapper.Map<IEnumerable<CarDTO>>(cars);
            return Ok(carDtos);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CarCreateDTO carDto)
        {
            var car = _mapper.Map<Car>(carDto);
            car.IsAvailable = true;

            await _carRepository.AddAsync(car);
            var carResultDto = _mapper.Map<CarDTO>(car);

            return CreatedAtAction(nameof(GetById), new { id = car.Id }, carResultDto);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] CarUpdateDTO carDto)
        {
            var existingCar = await _carRepository.GetByIdAsync(carDto.Id);
            if (existingCar == null)
                return NotFound();

            _mapper.Map(carDto, existingCar);
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