using Booking.DTO.Auth;
using Booking.Interfaces.Repositories;
using Booking.Interfaces.Services;
using Booking.Exceptions;
using Booking.Models;
using Booking.Enums;
using Booking.Constants;
using Microsoft.AspNetCore.Identity;
using Booking.Clients;


namespace Booking.Services
{
    public class AuthService(
        IAuthRepository _authRepository,
        IJwtService _jwtService,
        IEmailService _emailService) : IAuthService
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
            if (!await _authRepository.UpdateUserAsync(user))
                throw new InvalidOperationException("Failed to update profile.");

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

        public async Task<ApplicationUser> UpdateProfileAsync(ApplicationUser user, UpdateProfileDto dto)
        {
            if (!string.IsNullOrWhiteSpace(dto.FirstName))
                user.FirstName = dto.FirstName.Trim();

            if (!string.IsNullOrWhiteSpace(dto.LastName))
                user.LastName = dto.LastName.Trim();

            if (!string.IsNullOrWhiteSpace(dto.PhoneNumber))
                user.PhoneNumber = dto.PhoneNumber.Trim();

            if (!string.IsNullOrWhiteSpace(dto.Gender))
                user.Gender = dto.Gender.Trim();

            if (dto.DateOfBirth.HasValue)
                user.DateOfBirth = dto.DateOfBirth.Value;

            user.UpdatedAt = DateTime.UtcNow;

            if (!await _authRepository.UpdateUserAsync(user))
                throw new InvalidOperationException("Failed to update profile.");

            return user;
        }

        public async Task<IdentityResult> ChangePasswordAsync(ApplicationUser user, ChangePasswordDto dto)
        {
            return await _authRepository.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
        }

        public async Task<bool> SendResetPasswordEmailAsync(string email)
        {
            var user = await _authRepository.FindByEmailAsync(email);
            if (user is null) return false;

            await _authRepository.DeleteExistingCodesAsync(user.Id);

            var resetCode = new PasswordResetCode
            {
                UserId = user.Id,
                Code = GenerateCode(),
                ExpiresAt = DateTime.UtcNow.AddMinutes(15),
                IsUsed = false
            };

            await _authRepository.SaveResetCodeAsync(resetCode);
            // await _emailService.SendAsync(email, "Password Reset", $"Your reset code is: {resetCode.Code}");

            return true;
        }

        public async Task<bool> ValidateResetCodeAsync(string email, string code)
        {
            var user = await _authRepository.FindByEmailAsync(email);
            if (user is null) return false;

            var resetCode = await _authRepository.GetValidResetCodeAsync(user.Id, code);
            return resetCode is not null;
        }

        public async Task<bool> ResetPasswordAsync(string email, string newPassword)
        {
            var user = await _authRepository.FindByEmailAsync(email);
            if (user is null) return false;

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
            if (!result.Succeeded) return false;

            await _authRepository.MarkCodeAsUsedAsync(resetCode);
            return true;
        }

        private static string GenerateCode() =>
            Random.Shared.Next(100000, 999999).ToString();





    }
}
