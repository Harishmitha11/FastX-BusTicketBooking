using FastX_BusTicketBooking.API.Models.DTOs;
using FastX_BusTicketBooking.API.Models.Entities;
using FastX_BusTicketBooking.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FastX_BusTicketBooking.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingsController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [Authorize(Roles = "User,BusOperator")]
        [HttpPost("BookTicket")]
        public async Task<IActionResult> BookTicket([FromBody] BookingRequestDTO dto)
        {
            try
            {
                var loggedUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                if (dto.UserId != loggedUserId)
                    return Forbid("You are not authorized to book on behalf of another user.");

                var result = await _bookingService.BookTicket(dto);
                return Ok(new { message = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error booking ticket.", error = ex.Message });
            }
        }

        [Authorize(Roles = "Admin,BusOperator")]
        [HttpGet("GetAllBookings")]
        public async Task<IActionResult> GetAllBookings()
        {
            try
            {
                var result = await _bookingService.GetAllBookings();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching bookings.", error = ex.Message });
            }
        }

        [Authorize(Roles = "Admin,BusOperator")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookingById(int id)
        {
            try
            {
                var result = await _bookingService.GetBookingById(id);
                if (result == null)
                {
                    return NotFound(new { message = $"Booking with ID {id} not found." });
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching booking.", error = ex.Message });
            }
        }

        [Authorize(Roles = "User,BusOperator")]
        [HttpPut("{id}/Cancel")]
        public async Task<IActionResult> CancelBooking(int id)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var booking = await _bookingService.GetBookingById(id);
                if (booking == null)
                    return NotFound(new { message = "Booking not found." });

                if (User.IsInRole("User") && booking.UserId != userId)
                    return Forbid("You are not allowed to cancel another user's booking.");

                var result = await _bookingService.CancelBooking(id);
                return Ok(new { message = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error cancelling booking.", error = ex.Message });
            }
        }

        [Authorize(Roles = "User,BusOperator")]
        [HttpGet("MyBookings")]
        public async Task<IActionResult> GetMyBookings()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var result = await _bookingService.GetBookingsByUser(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching your bookings.", error = ex.Message });
            }
        }

        [Authorize(Roles = "User,BusOperator")]
        [HttpPut("CancelSeats")]
        public async Task<IActionResult> CancelSelectedSeats([FromBody] CancelSeatsDTO dto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

                var booking = await _bookingService.GetBookingById(dto.BookingId);

                if (booking == null)
                {
                    return NotFound(new { message = "Booking not found." });
                }
                if (User.IsInRole("User") && booking.UserId != userId)
                    return Forbid("You are not allowed to cancel seats for another user's booking.");

                var result = await _bookingService.CancelSelectedSeats(dto);
                return Ok(new { message = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error cancelling selected seats.", error = ex.Message });
            }
        }
    }
}
