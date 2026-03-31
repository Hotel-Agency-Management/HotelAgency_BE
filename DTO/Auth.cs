using System.ComponentModel.DataAnnotations;
using Booking.Enums;

namespace Booking.DTO.Auth
{

    public class RegisterRequest
    {
        [Required] public string FirstName { get; set; } = string.Empty;
        [Required] public string LastName { get; set; } = string.Empty;
        [Required][EmailAddress] public string Email { get; set; } = string.Empty;
        [Required] public string PhoneNumber { get; set; } = string.Empty;
        [Required] public string Password { get; set; } = string.Empty;
        [Required]public AccountType AccountType { get; set; }
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
    }

    public class RegisterResponseDto
    {
        public int UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }

}
