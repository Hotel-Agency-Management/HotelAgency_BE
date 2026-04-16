using Booking.DTO;
using Booking.Interfaces.Repositories;
using Booking.Interfaces.Services;
using Booking.Exceptions;
using Booking.Models;
using Booking.Enums;
using Booking.Constants;
using Microsoft.AspNetCore.Identity;
using Booking.Clients;
using System.Security.Cryptography;
using Booking.Utils;
using Booking.Factories;


namespace Booking.Services
{
    public class AuthService(
        IAuthRepository _authRepository,
        IJwtService _jwtService,
        IEmailService _emailService,
        IEmailJobService _emailJobService,
        IRegistrationStrategyFactory _strategyFactory,
        IProfileStrategyFactory _profileFactory,
        IAgencyRepository _agencyRepository) : IAuthService
    {
        public async Task<AuthResponseDto?> LoginAsync(LoginDto loginDto)
        {
            var user = await _authRepository.FindByEmailAsync(loginDto.Email)
                ?? throw new UserNotFoundException(loginDto.Email);

            if (!user.EmailConfirmed)
                throw new EmailNotConfirmedException(loginDto.Email);

            var isPasswordValid = await _authRepository.CheckPasswordAsync(user, loginDto.Password);
            if (!isPasswordValid)
                throw new InvalidCredentialsException();

            var role = await _authRepository.GetRoleAsync(user);


            Agency? agency = null;
            AgencyStatus? agencyStatus = null;

            if (role != Roles.Customer && role != Roles.SuperAdmin)
            {
                if (!user.AgencyId.HasValue)
                    throw new Exception("AgencyId is required.");

                agency = await _agencyRepository.GetByIdAsync(user.AgencyId.Value);
                if (agency == null)
                    throw new AgencyNotFoundException(user.Id);

                agencyStatus = agency.Status;

                if (agencyStatus == AgencyStatus.Pending)
                    throw new AgencyUnderReviewException();
            }

            await _authRepository.DeleteUserRefreshTokensAsync(user.Id);
            var token = _jwtService.GenerateToken(user, role);

            var refreshToken = new RefreshToken
            {
                UserId = user.Id,
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                IsRevoked = false
            };

            await _authRepository.SaveRefreshTokenAsync(refreshToken);

            user.LastLogin = DateTime.UtcNow;
            if (!await _authRepository.UpdateUserAsync(user))
                throw new InvalidOperationException("Failed to update profile.");

            return new AuthResponseDto
            {
                Token = token,
                Email = user.Email ?? string.Empty,
                RefreshToken = refreshToken.Token,
                FirstName = user.FirstName ?? string.Empty,
                LastName = user.LastName ?? string.Empty,
                Role = role,
                AgencyStatus = agencyStatus,
                AgencyId = agency?.Id

            };
        }

        public async Task<RegisterResultDto> RegisterAsync(RegisterRequest request)
        {
            if (await _authRepository.FindByEmailAsync(request.Email.Trim()) is not null)
                throw new EmailAlreadyExistsException(request.Email);

            var strategy = _strategyFactory.GetStrategy(request.AccountType);
            var user = await strategy.ExecuteAsync(request);
            var role = await _authRepository.GetRoleAsync(user);
            var token = _jwtService.GenerateToken(user, role);

            var refreshToken = new RefreshToken
            {
                UserId = user.Id,
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                IsRevoked = false
            };

            await _authRepository.SaveRefreshTokenAsync(refreshToken);

            await SendVerificationEmailAsync(user);
            return new RegisterResultDto
            {
                User = user,
                Token = token,
                RefreshToken = refreshToken.Token,
                Role = role
            };
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
            var user = await _authRepository.FindByEmailAsync(email)
                ?? throw new UserNotFoundException(email);

            await _authRepository.DeleteExistingCodesAsync(user.Id);

            var resetCode = new PasswordResetCode
            {
                UserId = user.Id,
                Code = Random.Shared.Next(100000, 999999).ToString(),
                ExpiresAt = DateTime.UtcNow.AddMinutes(15),
                IsUsed = false
            };

            await _authRepository.SaveResetCodeAsync(resetCode);

            var template = await _emailService.LoadTemplateAsync("verification-code-template.html");

            var userName = string.IsNullOrWhiteSpace($"{user.FirstName} {user.LastName}".Trim())
                ? "User"
                : $"{user.FirstName} {user.LastName}".Trim();

            var htmlBody = _emailService.RenderTemplate(template, new Dictionary<string, string>
                {
                    { "USER_NAME", userName },
                    { "RESET_CODE", resetCode.Code },
                    { "EXPIRATION_TIME", "15 minutes" },
                    { "HELP_LINK", "http://localhost:3000/help" },
                    { "SUPPORT_LINK", "http://localhost:3000/support" },
                    { "PRIVACY_LINK", "http://localhost:3000/privacy" },
                    { "AGENCY_NAME", "HotelAgency" }
                });

            var plainText = string.Format(
                EmailTemplates.ResetPassword,
                userName,
                resetCode.Code
            );

            await _emailService.SendEmailAsync(
                email,
                "Password Reset Verification Code",
                plainText,
                htmlBody
            );

            return true;
        }

        public async Task<bool> ValidateResetCodeAsync(string email, string code)
        {
            var user = await _authRepository.FindByEmailAsync(email)
                ?? throw new UserNotFoundException(email);

            var resetCode = await _authRepository.GetValidResetCodeAsync(user.Id, code);
            return resetCode is not null;
        }

        public async Task ResetPasswordAsync(string email, string code, string newPassword)
        {
            var user = await _authRepository.FindByEmailAsync(email)
                ?? throw new UserNotFoundException(email);

            var resetCode = await _authRepository.GetValidResetCodeAsync(user.Id, code)
                ?? throw new InvalidResetCodeException();

            var result = await _authRepository.ResetPasswordAsync(user, newPassword);
            if (!result.Succeeded)
                throw new InvalidOperationException("Failed to reset password");

            await _authRepository.MarkCodeAsUsedAsync(resetCode);
        }

        public async Task<RefreshTokenResponseDto> RefreshTokenAsync(string token)
        {
            var refreshToken = await _authRepository.GetValidRefreshTokenAsync(token)
                ?? throw new InvalidRefreshTokenException();

            var user = await _authRepository.FindByIdAsync(refreshToken.UserId)
                ?? throw new UserNotFoundException($"ID: {refreshToken.UserId}");

            await _authRepository.RevokeRefreshTokenAsync(refreshToken);

            var role = await _authRepository.GetRoleAsync(user);
            var newAccessToken = _jwtService.GenerateToken(user, role);

            var newRefreshToken = new RefreshToken
            {
                UserId = user.Id,
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                IsRevoked = false
            };

            await _authRepository.SaveRefreshTokenAsync(newRefreshToken);

            return new RefreshTokenResponseDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken.Token
            };
        }

        public async Task LogoutAsync(int userId)
        {
            await _authRepository.DeleteUserRefreshTokensAsync(userId);
        }

        public async Task SendVerificationEmailAsync(ApplicationUser user)
        {
            if (user.EmailConfirmed)
                return;

            await _authRepository.DeleteExistingEmailVerificationTokensAsync(user.Id);

            var rawToken = AuthUtils.GenerateSecureToken();
            var hashedToken = AuthUtils.HashToken(rawToken);

            var verificationToken = new EmailVerificationToken
            {
                UserId = user.Id,
                TokenHash = hashedToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(15),
                IsUsed = false
            };

            await _authRepository.SaveEmailVerificationTokenAsync(verificationToken);

            var verificationLink =
                $"http://localhost:3000/verify-email?userId={user.Id}&token={Uri.EscapeDataString(rawToken)}";

            await _emailJobService.EnqueueVerificationEmailAsync(user, verificationLink);
        }

        public async Task VerifyEmailAsync(int userId, string token)
        {
            var user = await _authRepository.FindByIdAsync(userId)
                ?? throw new UserNotFoundException($"ID: {userId}");

            if (user.EmailConfirmed)
                return;

            var tokenHash = AuthUtils.HashToken(token);

            var verificationToken = await _authRepository
                .GetValidEmailVerificationTokenAsync(user.Id, tokenHash)
                ?? throw new InvalidOperationException("Invalid or expired token");

            var confirmed = await _authRepository.ConfirmEmailAsync(user);

            if (!confirmed)
                throw new InvalidOperationException("Failed to confirm email");

            await _authRepository.MarkEmailVerificationTokenAsUsedAsync(verificationToken);
        }

        public async Task ResendVerificationEmailAsync(string email)
        {
            var user = await _authRepository.FindByEmailAsync(email)
                ?? throw new UserNotFoundException(email);

            if (user.EmailConfirmed)
                throw new InvalidOperationException("Email already verified");

            await SendVerificationEmailAsync(user);
        }

        public Task<BaseProfileResponseDto> GetProfileAsync(ApplicationUser user, string role)
        {
            var strategy = _profileFactory.Create(role);
            return strategy.BuildProfileAsync(user);
        }

    }
}
