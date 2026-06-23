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
        IEmailVerificationService _emailVerificationService,
        IEmailService _emailService,
        IEmailJobService _emailJobService,
        IAppLinkService _appLinkService,
        IRegistrationStrategyFactory _strategyFactory,
        IProfileStrategyFactory _profileFactory,
        IAgencyRepository _agencyRepository,
        IHotelRepository _hotelRepository,
        ILoginResponseStrategyFactory _loginResponseStrategyFactory,
        INotificationService _notificationService,
        ILogger<AuthService> _logger) : IAuthService
    {
        public async Task<AuthResponseDto?> LoginAsync(LoginDto loginDto)
        {
            var user = await ValidateUserCredentialsAsync(loginDto);
            var role = await _authRepository.GetRoleAsync(user);
            var (agency, agencyStatus) = await ResolveAgencyContextAsync(user, role);
            var hotel = await ResolveHotelContextAsync(user, role);
            var (token, refreshToken) = await GenerateAndSaveTokensAsync(user, role);

            _logger.LogInformation("User {Email} logged in successfully with role {Role}", loginDto.Email, role);
            var strategy = _loginResponseStrategyFactory.GetStrategy(role);
            return strategy.BuildResponse(user, role, token, refreshToken.Token, agency, agencyStatus, hotel);
        }

        private async Task<ApplicationUser> ValidateUserCredentialsAsync(LoginDto loginDto)
        {
            var user = await _authRepository.FindByEmailAsync(loginDto.Email)
                ?? throw new UserNotFoundException(loginDto.Email);

            if (!user.EmailConfirmed)
                throw new EmailNotConfirmedException(loginDto.Email);

            var isPasswordValid = await _authRepository.CheckPasswordAsync(user, loginDto.Password);
            if (!isPasswordValid)
                throw new InvalidCredentialsException();

            return user;
        }

        private async Task<(Agency? agency, AgencyStatus? agencyStatus)> ResolveAgencyContextAsync(ApplicationUser user, string role)
        {
            if (role == Roles.Customer || role == Roles.SuperAdmin)
                return (null, null);

            if (!user.AgencyId.HasValue)
                throw new Exception("AgencyId is required.");

            var agency = await _agencyRepository.GetByIdAsync(user.AgencyId.Value)
                ?? throw new AgencyNotFoundException(user.AgencyId.Value);

            if (agency.Status == AgencyStatus.Pending)
                throw new AgencyUnderReviewException();

            return (agency, agency.Status);
        }

        private async Task<Hotel?> ResolveHotelContextAsync(ApplicationUser user, string role)
        {
            if (role == Roles.Customer || role == Roles.SuperAdmin || role == Roles.AgencyOwner)
                return null;

            if (!user.HotelId.HasValue)
                throw new Exception("HotelId is required.");

            return await _hotelRepository.GetByIdAsync(user.HotelId.Value)
                ?? throw new HotelNotFoundException(user.HotelId.Value);
        }

        private async Task<(string token, RefreshToken refreshToken)> GenerateAndSaveTokensAsync(ApplicationUser user, string role)
        {
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

            return (token, refreshToken);
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
            _logger.LogInformation("User {Email} registered successfully with role {Role}", request.Email, role);
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

            _logger.LogInformation("Profile updated for user {UserId}", user.Id);
            return user;
        }

        public async Task<IdentityResult> ChangePasswordAsync(ApplicationUser user, ChangePasswordDto dto)
        {
            var result = await _authRepository.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
            if (result.Succeeded)
                await _notificationService.CreateAsync(new CreateNotificationRequest
                {
                    UserId = user.Id,
                    Title = "Password Changed",
                    Message = "Your password has been changed successfully. If you did not make this change, please contact support immediately.",
                    Type = NotificationType.System
                });
            return result;
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
                    { "HELP_LINK", _appLinkService.GetHelpLink() },
                    { "SUPPORT_LINK", _appLinkService.GetSupportLink() },
                    { "PRIVACY_LINK", _appLinkService.GetPrivacyLink() },
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

            _logger.LogInformation("Password reset code sent for {Email}", email);
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
            _logger.LogInformation("Password reset completed for {Email}", email);

            await _notificationService.CreateAsync(new CreateNotificationRequest
            {
                UserId = user.Id,
                Title = "Password Reset",
                Message = "Your password has been reset successfully. If you did not request this, please contact support immediately.",
                Type = NotificationType.System
            });
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

            _logger.LogInformation("Token refreshed for user {UserId}", refreshToken.UserId);
            return new RefreshTokenResponseDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken.Token
            };
        }

        public async Task LogoutAsync(int userId)
        {
            await _authRepository.DeleteUserRefreshTokensAsync(userId);
            _logger.LogInformation("User {UserId} logged out", userId);
        }

        public async Task SendVerificationEmailAsync(ApplicationUser user)
        {
            if (user.EmailConfirmed)
                return;

            var link = await _emailVerificationService.GenerateVerificationLinkAsync(user);
            await _emailJobService.EnqueueVerificationEmailAsync(user, link);
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
            _logger.LogInformation("Email verified for user {UserId}", userId);

            await _notificationService.CreateAsync(new CreateNotificationRequest
            {
                UserId = userId,
                Title = "Email Verified",
                Message = "Your email has been verified. Welcome! Your account is now active.",
                Type = NotificationType.System
            });
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
            return _profileFactory.BuildProfileAsync(role, user);
        }

    }
}
