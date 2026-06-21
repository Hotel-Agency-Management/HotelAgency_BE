using Booking.Clients;
using Booking.Configurations;
using Booking.Constants;
using Booking.Enums;
using Booking.Exceptions;
using Booking.Interfaces.Repositories;
using Booking.Interfaces.Services;
using Booking.Models;
using Microsoft.Extensions.Options;

namespace Booking.Services
{
    public class CustomerAccountService(
        IAuthRepository _authRepository,
        IOptions<AuthSettings> authSettings,
        IEmailVerificationService _emailVerificationService,
        IEmailJobService _emailJobService,
        ILogger<CustomerAccountService> _logger) : ICustomerAccountService
    {
        private readonly AuthSettings _authSettings = authSettings.Value;

        public async Task<int?> EnsureCustomerAsync(
            ReservationSource source,
            int? customerId,
            string guestEmail,
            string guestFullName,
            string guestPhone)
        {
            if (source != ReservationSource.WalkIn && source != ReservationSource.Phone)
                return customerId;

            var existing = await _authRepository.FindByEmailAsync(guestEmail);
            if (existing is not null)
            {
                _logger.LogDebug("Existing customer found for {Email}", guestEmail);
                return existing.Id;
            }

            var nameParts = guestFullName.Trim().Split(' ', 2);
            var firstName = nameParts[0];
            var lastName = nameParts.Length > 1 ? nameParts[1] : string.Empty;

            var user = new ApplicationUser
            {
                UserName = guestEmail,
                Email = guestEmail,
                FirstName = firstName,
                LastName = lastName,
                PhoneNumber = guestPhone,
                EmailConfirmed = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var result = await _authRepository.CreateUserAsync(user, _authSettings.DefaultPassword);
            if (!result.Succeeded)
                throw new BadRequestException("Failed to create customer account.");

            await _authRepository.AddToRoleAsync(user, Roles.Customer);

            var verifyLink = await _emailVerificationService.GenerateVerificationLinkAsync(user);
            await _emailJobService.EnqueueNewCustomerAccountEmailAsync(user, verifyLink, _authSettings.DefaultPassword);

            _logger.LogInformation("Customer account created for {Email}", guestEmail);
            return user.Id;
        }
    }
}
