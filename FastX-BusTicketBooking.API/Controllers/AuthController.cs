using FastX_BusTicketBooking.API.Models.DTOs;
using FastX_BusTicketBooking.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace FastX_BusTicketBooking.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDTO)
        {
            try
            {
                var result = await _authService.Register(registerDTO);
                if (result.Contains("already"))
                {
                    return BadRequest(new { message = result });
                }

                return Ok(new { message = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred during registration.", error = ex.Message });
            }
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            try
            {
                var result = await _authService.Login(loginDTO);
                if (result == "Invalid Credentials.")
                {
                    return Unauthorized(new { message = result });
                }

                return Ok(new { token = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred during login.", error = ex.Message });
            }
        }
    }
}
