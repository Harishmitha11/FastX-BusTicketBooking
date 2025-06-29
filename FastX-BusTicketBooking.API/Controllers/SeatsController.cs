using FastX_BusTicketBooking.API.Models.DTOs;
using FastX_BusTicketBooking.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FastX_BusTicketBooking.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeatsController : ControllerBase
    {
        private readonly ISeatService _seatService;

        public SeatsController(ISeatService seatService)
        {
            _seatService = seatService;
        }

        [Authorize]
        [HttpGet("GetSeatsByRoute/{routeId}")]
        public async Task<IActionResult> GetSeatsByRoute(int routeId)
        {
            try
            {
                var seats = await _seatService.GetSeatsByRoute(routeId);
                return Ok(seats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching seats", error = ex.Message });
            }
        }
        [Authorize(Roles ="User,BusOperator")]
        [HttpPut("BookSeats/{routeId}/{count}")]
        public async Task<IActionResult> BookSeats(int routeId, int count)
        {
            try
            {
                var result = await _seatService.BookSeats(routeId, count);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error booking seats", error = ex.Message });
            }
        }
    }
}
