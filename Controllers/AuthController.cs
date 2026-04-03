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
                DateOfBirth = result.DateOfBirth,
                Gender = result.Gender
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
                DateOfBirth = user.DateOfBirth,
                Gender = user.Gender
            });
        }


        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
                return Unauthorized();

            var result = await _authService.ChangePasswordAsync(user, dto);

            if (!result.Succeeded)
            {
                return BadRequest(new ChangePasswordResponseDto
                {
                    Success = false,
                    Message = "Password change failed",
                    Errors = result.Errors.Select(e => e.Description).ToList()
                });
            }

            return Ok(new ChangePasswordResponseDto
            {
                Success = true,
                Message = "Password changed successfully"
            });
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user is null)
                return NotFound(new PasswordResetResponseDto
                {
                    Success = false,
                    Message = "No account associated with this email"
                });

            var sent = await _authService.SendResetPasswordEmailAsync(dto.Email);
            if (!sent)
                return BadRequest(new PasswordResetResponseDto
                {
                    Success = false,
                    Message = "Failed to send reset email, please try again"
                });

            return Ok(new PasswordResetResponseDto
            {
                Success = true,
                Message = "Password reset email sent"
            });
        }

        [HttpPost("validate-reset-code")]
        public async Task<IActionResult> ValidateResetCode([FromBody] ValidateResetCodeDto dto)
        {
            var isValid = await _authService.ValidateResetCodeAsync(dto.Email, dto.Code);
            if (!isValid)
                return BadRequest(new PasswordResetResponseDto
                {
                    Success = false,
                    Message = "Invalid or expired reset code"
                });

            return Ok(new PasswordResetResponseDto
            {
                Success = true,
                Message = "Code is valid"
            });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            await _authService.ResetPasswordAsync(
                dto.Email,
                dto.Code,
                dto.NewPassword
            );

            return Ok(new PasswordResetResponseDto
            {
                Success = true,
                Message = "Password reset successfully"
            });
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto dto)
        {
            var result = await _authService.RefreshTokenAsync(dto.RefreshToken);
            return Ok(result);
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
                return Unauthorized();

            await _authService.LogoutAsync(user.Id);
            return NoContent();
        }

        [HttpGet("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromQuery] int userId, [FromQuery] string token)
        {
            await _authService.VerifyEmailAsync(userId, token);

            return Ok(new
            {
                Success = true,
                Message = "Email verified successfully"
            });
        }

        [HttpPost("resend-verification-email")]
        public async Task<IActionResult> ResendVerificationEmail([FromBody] ResendVerificationEmailDto dto)
        {
            await _authService.ResendVerificationEmailAsync(dto.Email);

            return Ok(new
            {
                Success = true,
                Message = "Verification email sent"
            });
        }


    }
}
