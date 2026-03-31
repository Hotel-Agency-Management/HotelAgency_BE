using Booking.DTO.Auth;
using Booking.Interfaces.Repositories;
using Booking.Interfaces.Services;
using Booking.Exceptions;
using Booking.Models;
using Booking.Enums;
using Booking.Constants;

namespace Booking.Services
{
    public class AuthService(IAuthRepository _authRepository, IJwtService _jwtService) : IAuthService
    {
        public async Task<AuthResponseDto?> LoginAsync(LoginDto loginDto)
        {
            var user = await _authRepository.FindByEmailAsync(loginDto.Email)
                ?? throw new UserNotFoundException(loginDto.Email);

            var isPasswordValid = await _authRepository.CheckPasswordAsync(user, loginDto.Password);

            if (!isPasswordValid)
                throw new InvalidCredentialsException();

            var role = await _authRepository.GetRoleAsync(user);
            var token = _jwtService.GenerateToken(user, role);
            user.LastLogin = DateTime.UtcNow;

            return new AuthResponseDto
            {
                Token = token,
                Email = user.Email ?? string.Empty,
                FirstName = user.FirstName ?? string.Empty,
                LastName = user.LastName ?? string.Empty,
                Role = role
            };
        }

        public async Task<ApplicationUser> RegisterAsync(RegisterRequest request)
        {

            var email = request.Email.Trim();

            var existingUser = await _authRepository.FindByEmailAsync(email);
            if (existingUser != null)
                throw new EmailAlreadyExistsException(email);

            var user = new ApplicationUser
            {
                UserName = email,
                EmailConfirmed = false,
                Email = email,
                PhoneNumber = request.PhoneNumber,
                FirstName = request.FirstName,
                LastName = request.LastName,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var result = await _authRepository.CreateUserAsync(user, request.Password);

            if (!result.Succeeded)
                throw new RegistrationFailedException("Registration failed.");

            string role = request.AccountType switch
            {
                AccountType.Customer => Roles.Customer,
                AccountType.AgencyOwner => Roles.AgencyOwner,
                _ => throw new InvalidOperationException("Invalid account type.")
            };

            var roleResult = await _authRepository.AddToRoleAsync(user, role);

            if (!roleResult.Succeeded)
                throw new RegistrationFailedException("User created but assigning role failed.");

            return user;
        }

        public async Task UpdateProfile(ApplicationUser user, UpdateProfileRequest request)
        {
            if (request.FirstName != null)
                user.FirstName = request.FirstName;

            if (request.LastName != null)
                user.LastName = request.LastName;

            if (request.PhoneNumber != null)
                user.PhoneNumber = request.PhoneNumber;

            user.UpdatedAt = DateTime.UtcNow;

            await _authRepository.UpdateUserAsync(user);
        }






    }
}
