using Booking.Constants;
using Booking.DTO;
using Booking.Enums;
using Booking.Exceptions;
using Booking.Interfaces.Repositories;
using Booking.Interfaces.Services;
using Booking.Models;
using Booking.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Booking.Clients;

namespace Booking.Strategies
{
    public class AgencyOwnerRegistrationStrategy(
        IAuthRepository _authRepository,
        IAgencyRepository _agencyRepository,
        ApplicationDbContext _dbContext,
        IEmailJobService _emailJobService,
        INotificationService _notificationService
    ) : IRegistrationStrategy
    {
        public async Task<ApplicationUser> ExecuteAsync(RegisterRequest request)
        {
            var agencyRequest = (AgencyOwnerRegisterRequest)request;
            var strategy = _dbContext.Database.CreateExecutionStrategy();

            var user = await strategy.ExecuteAsync(async () =>
            {
                await using IDbContextTransaction transaction =
                    await _dbContext.Database.BeginTransactionAsync();

                try
                {
                    var u = BuildUser(agencyRequest);

                    var result = await _authRepository.CreateUserAsync(u, agencyRequest.Password);
                    if (!result.Succeeded)
                        throw new RegistrationFailedException("Registration failed.");

                    var roleResult = await _authRepository.AddToRoleAsync(u, Roles.AgencyOwner);
                    if (!roleResult.Succeeded)
                        throw new RegistrationFailedException("User created but assigning role failed.");

                    await BuildAgencyAsync(u, agencyRequest);

                    await transaction.CommitAsync();
                    return u;
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            });

            await _notificationService.NotifySuperAdminsAsync(
                NotificationTitles.NewAgencyRegistered,
                string.Format(NotificationMessages.NewAgencyRegistered, agencyRequest.AgencyName.Trim()),
                NotificationType.NewAgencyRegistered);

            return user;
        }

        private static ApplicationUser BuildUser(AgencyOwnerRegisterRequest request) => new()
        {
            UserName = request.Email.Trim(),
            Email = request.Email.Trim(),
            EmailConfirmed = false,
            PhoneNumber = request.PhoneNumber,
            FirstName = request.FirstName,
            LastName = request.LastName,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        private async Task BuildAgencyAsync(ApplicationUser user, AgencyOwnerRegisterRequest request)
        {
            var exists = await _agencyRepository.ExistsByNameAsync(request.AgencyName);
            if (exists)
                throw new AgencyAlreadyExistsException(request.AgencyName);

            var agency = new Agency
            {
                AgencyName = request.AgencyName.Trim(),
                Country = request.Country.Trim(),
                City = request.City.Trim(),
                Phone = request.Phone.Trim(),
                OwnerId = user.Id,
                Status = AgencyStatus.InComplete,
                PlanId = PlanConstant.FreePlanId,
                CreatedAt = DateTime.UtcNow
            };

            await _agencyRepository.AddAsync(agency);

            user.AgencyId = agency.Id;
            var updated = await _authRepository.UpdateUserAsync(user);
            if (!updated)
                throw new RegistrationFailedException("Failed to link agency to user.");

            await _emailJobService.EnqueueAgencyUnderReviewEmailAsync(user);
        }
    }
}
