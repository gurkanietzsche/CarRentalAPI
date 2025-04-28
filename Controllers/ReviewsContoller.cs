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
    public class ReviewsController : ControllerBase
    {
        private readonly ReviewRepository _reviewRepository;
        private readonly RentalRepository _rentalRepository;
        private readonly IMapper _mapper;

        public ReviewsController(ReviewRepository reviewRepository, RentalRepository rentalRepository, IMapper mapper)
        {
            _reviewRepository = reviewRepository;
            _rentalRepository = rentalRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var reviews = await _reviewRepository.GetAllWithDetailsAsync();
            var reviewDtos = _mapper.Map<IEnumerable<ReviewDTO>>(reviews);
            return Ok(reviewDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var review = await _reviewRepository.GetByIdWithDetailsAsync(id);
            if (review == null)
                return NotFound();

            var reviewDto = _mapper.Map<ReviewDTO>(review);
            return Ok(reviewDto);
        }

        [HttpGet("car/{carId}")]
        public async Task<IActionResult> GetCarReviews(int carId)
        {
            var reviews = await _reviewRepository.GetCarReviewsAsync(carId);
            var reviewDtos = _mapper.Map<IEnumerable<ReviewDTO>>(reviews);
            return Ok(reviewDtos);
        }

        [Authorize]
        [HttpGet("user")]
        public async Task<IActionResult> GetUserReviews()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var reviews = await _reviewRepository.GetUserReviewsAsync(userId);
            var reviewDtos = _mapper.Map<IEnumerable<ReviewDTO>>(reviews);
            return Ok(reviewDtos);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ReviewCreateDTO reviewDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Check if user has rented this car before
            var userRentals = await _rentalRepository.GetUserRentalsAsync(userId);
            var hasRentedCar = userRentals.Any(r => r.CarId == reviewDto.CarId && r.Status == "Completed");

            if (!hasRentedCar && !User.IsInRole("Admin"))
                return BadRequest(new { Message = "You can only review cars that you have rented" });

            // Check if rating is within valid range
            if (reviewDto.Rating < 1 || reviewDto.Rating > 5)
                return BadRequest(new { Message = "Rating must be between 1 and 5" });

            var review = _mapper.Map<Review>(reviewDto);
            review.UserId = userId;

            await _reviewRepository.AddAsync(review);
            var reviewResultDto = _mapper.Map<ReviewDTO>(review);

            return CreatedAtAction(nameof(GetById), new { id = review.Id }, reviewResultDto);
        }

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] ReviewUpdateDTO reviewDto)
        {
            var existingReview = await _reviewRepository.GetByIdAsync(reviewDto.Id);
            if (existingReview == null)
                return NotFound();

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (existingReview.UserId != userId && !User.IsInRole("Admin"))
                return Forbid();

            // Check if rating is within valid range
            if (reviewDto.Rating < 1 || reviewDto.Rating > 5)
                return BadRequest(new { Message = "Rating must be between 1 and 5" });

            _mapper.Map(reviewDto, existingReview);
            await _reviewRepository.UpdateAsync(existingReview);

            return NoContent();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var review = await _reviewRepository.GetByIdAsync(id);
            if (review == null)
                return NotFound();

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (review.UserId != userId && !User.IsInRole("Admin"))
                return Forbid();

            var result = await _reviewRepository.DeleteAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}