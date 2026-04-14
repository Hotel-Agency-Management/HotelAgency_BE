using Booking.DTO;
using Booking.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Booking.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Booking.Enums;
using Booking.Constants;
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

            var result = await _authService.RegisterAsync(request);
            var response = new RegisterResponseDto
            {
                UserId = result.User.Id,
                Email = result.User.Email!,
                Message = request.AccountType == AccountType.AgencyOwner
                    ? Messages.AgencyUnderReview
                    : Messages.VerifyEmail,
                AgencyId = request.AccountType == AccountType.AgencyOwner
                    ? result.User.AgencyId
                    : null,
                RefreshToken = result.RefreshToken,
                Token = result.Token,
                Role = result.Role
            };

            return Ok(response);
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
            if (user == null) return Unauthorized();

            var role = User.FindFirstValue(ClaimTypes.Role)
                ?? throw new InvalidOperationException("Role claim not found.");

            var result = await _authService.GetProfileAsync(user, role);

            return Ok(result);
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
                    Message = Messages.PasswordChangeFailed,
                    Errors = result.Errors.Select(e => e.Description).ToList()
                });
            }

            return Ok(new ChangePasswordResponseDto
            {
                Success = true,
                Message = Messages.PasswordChangedSuccessfully
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
                    Message = Messages.NoAccountWithEmail
                });

            var sent = await _authService.SendResetPasswordEmailAsync(dto.Email);
            if (!sent)
                return BadRequest(new PasswordResetResponseDto
                {
                    Success = false,
                    Message = Messages.FailedToSendResetEmail
                });

            return Ok(new PasswordResetResponseDto
            {
                Success = true,
                Message = Messages.PasswordResetEmailSent
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
                    Message = Messages.InvalidOrExpiredCode
                });

            return Ok(new PasswordResetResponseDto
            {
                Success = true,
                Message = Messages.CodeIsValid
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
                Message = Messages.PasswordResetSuccessfully
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
                Message = Messages.EmailVerifiedSuccessfully
            });
        }

        [HttpPost("resend-verification-email")]
        public async Task<IActionResult> ResendVerificationEmail([FromBody] ResendVerificationEmailDto dto)
        {
            await _authService.ResendVerificationEmailAsync(dto.Email);

            return Ok(new
            {
                Success = true,
                Message = Messages.VerificationEmailSent
            });
        }
    }
}
