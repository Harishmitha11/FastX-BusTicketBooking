using FastX_BusTicketBooking.API.Models.DTOs;
using FastX_BusTicketBooking.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using FastX_BusTicketBooking.API.Contexts;
using Microsoft.EntityFrameworkCore;

namespace FastX_BusTicketBooking.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RefundsController : ControllerBase
    {
        private readonly IRefundService _refundService;
        private readonly AppDbContext _context;

        public RefundsController(IRefundService refundService, AppDbContext context)
        {
            _refundService = refundService;
            _context = context;
        }

        [Authorize(Roles = "Admin,BusOperator")]
        [HttpPost("ProcessRefund")]
        public async Task<IActionResult> ProcessRefund(RefundDTO dto)
        {
            try
            {
                var result = await _refundService.ProcessRefund(dto);
                return Ok(new { message = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error processing refund", error = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("GetAllRefunds")]
        public async Task<IActionResult> GetAllRefunds()
        {
            try
            {
                var result = await _refundService.GetAllRefunds();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching refunds", error = ex.Message });
            }
        }

        [Authorize(Roles = "Admin,User,BusOperator")]
        [HttpGet("ByBooking/{bookingId}")]
        public async Task<IActionResult> GetRefundByBookingId(int bookingId)
        {
            try
            {
                var booking = await _context.Bookings.FindAsync(bookingId);
                if (booking == null)
                {
                    return NotFound(new { message = "Booking not found." });
                }

                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

                if (userRole != "Admin" && userIdClaim != null && booking.UserId != int.Parse(userIdClaim))
                {
                    return Forbid("You are not authorized to access this refund.");
                }

                var result = await _refundService.GetRefundByBookingId(bookingId);
                if (result == null)
                {
                    return NotFound(new { message = "Refund not yet processed or not available for this booking." });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching refund by booking", error = ex.Message });
            }
        }
    }
}
