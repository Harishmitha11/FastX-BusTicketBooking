using FastX_BusTicketBooking.API.Models.DTOs;
using FastX_BusTicketBooking.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FastX_BusTicketBooking.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class RoutesController : ControllerBase
    {
        private readonly IRouteService _routeService;

        public RoutesController(IRouteService routeService)
        {
            _routeService = routeService;
        }

        [HttpGet("GetAllRoutes")]
        public async Task<IActionResult> GetAllRoutes()
        {
            try
            {
                var routes = await _routeService.GetAllRoutes();
                return Ok(routes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching routes", error = ex.Message });
            }
        }

        [HttpGet("{id}/GetRouteById")]
        public async Task<IActionResult> GetRouteById(int id)
        {
            try
            {
                var route = await _routeService.GetRouteById(id);
                return route == null
            ? NotFound(new { message = "Route not found." })
            : Ok(route);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching route", error = ex.Message });
            }
        }

        [Authorize(Roles = "Admin,BusOperator")]
        [HttpPost("AddRoute")]
        public async Task<IActionResult> AddRoute(RouteDTO routeDTO)
        {
            try
            {
                var result = await _routeService.AddRoute(routeDTO);
                return Ok(new { message = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error adding route", error = ex.Message });
            }
        }

        [Authorize(Roles = "Admin,BusOperator")]
        [HttpPut("{id}/UpdateRoute")]
        public async Task<IActionResult> UpdateRoute(int id, RouteDTO routeDTO)
        {
            try
            {
                var result = await _routeService.UpdateRoute(id, routeDTO);
                return Ok(new { message = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating route", error = ex.Message });
            }
        }

        [Authorize(Roles = "Admin,BusOperator")]
        [HttpDelete("{id}/DeleteRoute")]
        public async Task<IActionResult> DeleteRoute(int id)
        {
            try
            {
                var result = await _routeService.DeleteRoute(id);
                return Ok(new { message = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error deleting route", error = ex.Message });
            }
        }

        [HttpGet("Search")]
        public async Task<IActionResult> SearchRoutes([FromQuery] string origin, [FromQuery] string destination, [FromQuery] DateTime travelDate)
        {
            try
            {
                var results = await _routeService.SearchRoutes(origin, destination, travelDate);
                return Ok(results);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error searching routes", error = ex.Message });
            }
        }
    }
}
