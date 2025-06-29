using FastX_BusTicketBooking.API.Models.DTOs;
using FastX_BusTicketBooking.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FastX_BusTicketBooking.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BusesController : ControllerBase
    {
        private readonly IBusService _busService;

        public BusesController(IBusService busService)
        {
            _busService = busService;
        }

        [HttpGet("GetAllBuses")]
        public async Task<IActionResult> GetAllBuses()
        {
            try
            {
                var buses = await _busService.GetAllBuses();
                return Ok(buses);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching all buses", error = ex.Message });
            }
        }

        [HttpGet("{id}/GetBusById")]
        public async Task<IActionResult> GetBusById(int id)
        {
            try
            {
                var bus = await _busService.GetBusById(id);
                if (bus == null)
                {
                    return NotFound(new { message = $"Bus with ID {id} not found." });
                }
                return Ok(bus);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching bus by ID", error = ex.Message });
            }
        }

        [Authorize(Roles = "Admin,BusOperator")]
        [HttpPost("AddBus")]
        public async Task<IActionResult> AddBus([FromBody] BusDTO busDTO)
        {
            try
            {
                var result = await _busService.AddBus(busDTO);
                return Ok(new { message = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error adding bus", error = ex.Message });
            }
        }

        [Authorize(Roles = "Admin,BusOperator")]
        [HttpPut("{id}/UpdateBus")]
        public async Task<IActionResult> UpdateBus(int id, [FromBody] BusDTO busDTO)
        {
            try
            {
                var result = await _busService.UpdateBus(id, busDTO);
                return Ok(new { message = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating bus", error = ex.Message });
            }
        }

        [Authorize(Roles = "Admin,BusOperator")]
        [HttpDelete("{id}/DeleteBus")]
        public async Task<IActionResult> DeleteBus(int id)
        {
            try
            {
                var result = await _busService.DeleteBus(id);
                return Ok(new { message = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error deleting bus", error = ex.Message });
            }
        }
    }
}
