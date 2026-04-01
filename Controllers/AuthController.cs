using Booking.DTO.Auth;
using Booking.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Booking.Models;
using Microsoft.AspNetCore.Identity;
namespace Booking.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(
        IAuthService _authService,
        UserManager<ApplicationUser> _userManager) : ControllerBase
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

        [HttpPatch("profile")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return Unauthorized();

            var result = await _authService.UpdateProfileAsync(user, dto);
            return Ok(new UpdateProfileResponseDto
            {
                Email = result.Email ?? string.Empty,
                FirstName = result.FirstName ?? string.Empty,
                LastName = result.LastName ?? string.Empty,
                PhoneNumber = result.PhoneNumber,
                UpdatedAt = result.UpdatedAt ?? DateTime.UtcNow,
                DateOfBirth = result.DateOfBirth
            });
        }


        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return Unauthorized();

            return Ok(new UpdateProfileResponseDto
            {
                Email = user.Email ?? string.Empty,
                FirstName = user.FirstName ?? string.Empty,
                LastName = user.LastName ?? string.Empty,
                PhoneNumber = user.PhoneNumber,
                UpdatedAt = user.UpdatedAt ?? DateTime.UtcNow,
                DateOfBirth = user.DateOfBirth
            });
        }


    }
}
