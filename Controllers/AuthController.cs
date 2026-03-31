using Booking.DTO.Auth;
using Booking.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
namespace Booking.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(IAuthService _authService) : ControllerBase
    {
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.LoginAsync(loginDto);
            return Ok(result);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _authService.RegisterAsync(request);

            return Ok(new RegisterResponseDto
            {
                UserId = user.Id,
                Email = user.Email!,
                Message = "User created successfully"
            });
        }


    }
}
