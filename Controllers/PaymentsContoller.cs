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
    public class PaymentsController : ControllerBase
    {
        private readonly GenericRepository<Payment> _paymentRepository;
        private readonly RentalRepository _rentalRepository;
        private readonly IMapper _mapper;

        public PaymentsController(
            GenericRepository<Payment> paymentRepository,
            RentalRepository rentalRepository,
            IMapper mapper)
        {
            _paymentRepository = paymentRepository;
            _rentalRepository = rentalRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var payments = await _paymentRepository.GetAllAsync();
            var paymentDtos = _mapper.Map<IEnumerable<PaymentDTO>>(payments);
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

            var paymentDto = _mapper.Map<PaymentDTO>(payment);
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

            var payment = _mapper.Map<Payment>(paymentDto);
            payment.PaymentDate = DateTime.Now;
            payment.Status = "Completed"; // In real world, this would be based on payment processor response
            payment.TransactionId = Guid.NewGuid().ToString();

            await _paymentRepository.AddAsync(payment);

            // Update rental status
            rental.Status = "Paid";
            await _rentalRepository.UpdateAsync(rental);

            var paymentResultDto = _mapper.Map<PaymentDTO>(payment);
            return CreatedAtAction(nameof(GetById), new { id = payment.Id }, paymentResultDto);
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update([FromBody] PaymentUpdateDTO paymentDto)
        {
            var existingPayment = await _paymentRepository.GetByIdAsync(paymentDto.Id);
            if (existingPayment == null)
                return NotFound();

            _mapper.Map(paymentDto, existingPayment);
            existingPayment.ModifiedDate = DateTime.Now;

            await _paymentRepository.UpdateAsync(existingPayment);
            return NoContent();
        }
    }
}