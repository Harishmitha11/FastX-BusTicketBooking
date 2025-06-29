using FastX_BusTicketBooking.API.Models.DTOs;
using FastX_BusTicketBooking.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using FastX_BusTicketBooking.API.Contexts;

namespace FastX_BusTicketBooking.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly AppDbContext _context;

        public PaymentsController(IPaymentService paymentService, AppDbContext context)
        {
            _paymentService = paymentService;
            _context = context;
        }

        [Authorize(Roles = "User,BusOperator")]
        [HttpPost("Process")]
        public async Task<IActionResult> ProcessPayment(PaymentDTO dto)
        {
            try
            {
                var booking = await _context.Bookings.FindAsync(dto.BookingId);
                if (booking == null)
                {
                    return NotFound(new { message = "Booking not found" });
                }

                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                if (booking.UserId != userId)
                {
                    return Forbid("You are not authorized to process payment for this booking.");
                }

                var result = await _paymentService.ProcessPayment(dto);
                return Ok(new { message = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error processing payment", error = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllPayments()
        {
            try
            {
                var result = await _paymentService.GetAllPayments();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching payments", error = ex.Message });
            }
        }

        [Authorize(Roles = "User,BusOperator,Admin")]
        [HttpGet("{bookingId}/GetByBooking")]
        public async Task<IActionResult> GetPaymentByBooking(int bookingId)
        {
            try
            {
                var booking = await _context.Bookings.FindAsync(bookingId);
                if (booking == null)
                {
                    return NotFound(new { message = "The booking you're looking for does not exist." });
                }

                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

                if (userRole == "User" && booking.UserId != userId)
                {
                    return Forbid("Access Denied. As a user, you can only view your own payment details.");
                }
                
               

                var result = await _paymentService.GetPaymentByBookingId(bookingId);
                if (result == null)
                {
                    return NotFound(new { message = "No payment has been made for this booking yet." });
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching payment details.", error = ex.Message });
            }
        }
    }
}
