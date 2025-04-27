using System.Security.Claims;
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
    public class PaymentsController : ControllerBase
    {
        private readonly GenericRepository<Payment> _paymentRepository;
        private readonly RentalRepository _rentalRepository;

        public PaymentsController(
            GenericRepository<Payment> paymentRepository,
            RentalRepository rentalRepository)
        {
            _paymentRepository = paymentRepository;
            _rentalRepository = rentalRepository;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var payments = await _paymentRepository.GetAllAsync();
            var paymentDtos = payments.Select(p => new PaymentDTO
            {
                Id = p.Id,
                Amount = p.Amount,
                PaymentDate = p.PaymentDate,
                PaymentMethod = p.PaymentMethod,
                Status = p.Status,
                TransactionId = p.TransactionId,
                RentalId = p.RentalId
            });

            return Ok(paymentDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var payment = await _paymentRepository.GetByIdAsync(id);
            if (payment == null)
                return NotFound();

            // Check permission
            var rental = await _rentalRepository.GetByIdWithDetailsAsync(payment.RentalId);
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (rental.UserId != userId && !User.IsInRole("Admin"))
                return Forbid();

            var paymentDto = new PaymentDTO
            {
                Id = payment.Id,
                Amount = payment.Amount,
                PaymentDate = payment.PaymentDate,
                PaymentMethod = payment.PaymentMethod,
                Status = payment.Status,
                TransactionId = payment.TransactionId,
                RentalId = payment.RentalId
            };

            return Ok(paymentDto);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PaymentCreateDTO paymentDto)
        {
            var rental = await _rentalRepository.GetByIdWithDetailsAsync(paymentDto.RentalId);
            if (rental == null)
                return NotFound(new { Message = "Rental not found" });

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (rental.UserId != userId && !User.IsInRole("Admin"))
                return Forbid();

            // In a real-world scenario, here you would integrate with a payment processor
            // For simplicity, we'll just create a payment record

            var payment = new Payment
            {
                Amount = paymentDto.Amount,
                PaymentDate = DateTime.Now,
                PaymentMethod = paymentDto.PaymentMethod,
                Status = "Completed", // In real world, this would be based on payment processor response
                TransactionId = Guid.NewGuid().ToString(),
                RentalId = paymentDto.RentalId
            };

            await _paymentRepository.AddAsync(payment);

            // Update rental status
            rental.Status = "Paid";
            await _rentalRepository.UpdateAsync(rental);

            return CreatedAtAction(nameof(GetById), new { id = payment.Id }, payment);
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update([FromBody] PaymentUpdateDTO paymentDto)
        {
            var existingPayment = await _paymentRepository.GetByIdAsync(paymentDto.Id);
            if (existingPayment == null)
                return NotFound();

            existingPayment.Status = paymentDto.Status;
            existingPayment.TransactionId = paymentDto.TransactionId;
            existingPayment.ModifiedDate = DateTime.Now;

            await _paymentRepository.UpdateAsync(existingPayment);
            return NoContent();
        }
    }
}