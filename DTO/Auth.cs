using System.ComponentModel.DataAnnotations;
using Booking.Enums;
using Booking.Models;

namespace Booking.DTO
{

    public abstract class RegisterRequest
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string PhoneNumber { get; set; }
        public abstract AccountType AccountType { get; }
    }

    public class CustomerRegisterRequest : RegisterRequest
    {
        public override AccountType AccountType => AccountType.Customer;
    }

    public class AgencyOwnerRegisterRequest : RegisterRequest
    {
        public override AccountType AccountType => AccountType.AgencyOwner;
        public required string AgencyName { get; set; }
        public required string Country { get; set; }
        public required string City { get; set; }
        public required string Phone { get; set; }
    }



    public class LoginDto
    {
        [Required][EmailAddress] public string Email { get; set; } = string.Empty;

        [Required] public string Password { get; set; } = string.Empty;
    }


    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public AgencyStatus? AgencyStatus { get; set; }
        public int? AgencyId { get; set; }
        public int? HotelId { get; set; }

    }


    public class RegisterResponseDto
    {
        public int UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public int? AgencyId { get; set; }
        public string RefreshToken { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;

    }
    public class RegisterResultDto
    {
        public ApplicationUser User { get; set; } = default!;
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }



    public class UpdateProfileDto
    {
        public string? FirstName { get; set; } = string.Empty;
        public string? LastName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; } = string.Empty;
    }


    public class UpdateProfileResponseDto
    {
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
    }

    public class BaseProfileResponseDto
    {
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public BaseProfileResponseDto(ApplicationUser user)
        {
            Email = user.Email ?? string.Empty;
            FirstName = user.FirstName ?? string.Empty;
            LastName = user.LastName ?? string.Empty;
            PhoneNumber = user.PhoneNumber;
            UpdatedAt = user.UpdatedAt ?? DateTime.UtcNow;
            DateOfBirth = user.DateOfBirth;
            Gender = user.Gender;
        }
    }

    public class AgencyOwnerProfileResponseDto : BaseProfileResponseDto
    {
        public AgencyInfoDto Agency { get; set; }

        public AgencyOwnerProfileResponseDto(ApplicationUser user) : base(user)
        {
            Agency = new AgencyInfoDto(user.Agency!);
        }
    }

    public class AgencyMemberProfileResponseDto : AgencyOwnerProfileResponseDto
    {
        public AgencyMemberProfileResponseDto(ApplicationUser user) : base(user) { }
    }
    
    public class HotelStaffProfileResponseDto : AgencyMemberProfileResponseDto
    {
        public HotelInfoDto Hotel { get; set; }

        public HotelStaffProfileResponseDto(ApplicationUser user) : base(user)
        {
            Hotel = new HotelInfoDto(user.Hotel!);
        }
    }


    public class ChangePasswordDto
    {
        [Required(ErrorMessage = "Current Password is required")]
        public required string CurrentPassword { get; set; } = string.Empty;


        [Required(ErrorMessage = "New Password is required")]
        [StringLength(15, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 15 characters ")]
        public required string NewPassword { get; set; } = string.Empty;

    }


    public class ChangePasswordResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<string> Errors { get; set; } = [];
    }


    public class ForgotPasswordDto
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }
    }


    public class ValidateResetCodeDto
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        public required string Code { get; set; }
    }


    public class ResetPasswordDto
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        [StringLength(15, MinimumLength = 8)]
        public required string NewPassword { get; set; }

        [Required]
        public required string Code { get; set; }

    }


    public class PasswordResetResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }


    public class RefreshTokenDto
    {
        [Required]
        public required string RefreshToken { get; set; }
    }


    public class RefreshTokenResponseDto
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }


    public class ResendVerificationEmailDto
    {
        public string Email { get; set; } = string.Empty;
    }

}
