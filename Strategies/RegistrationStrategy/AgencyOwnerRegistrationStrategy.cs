using Booking.Constants;
using Booking.DTO;
using Booking.Exceptions;
using Booking.Interfaces.Repositories;
using Booking.Models;
using Booking.Enums;
using Booking.Data;
using Microsoft.EntityFrameworkCore.Storage;
using Booking.Clients;

namespace Booking.Strategies
{
    public class AgencyOwnerRegistrationStrategy(
        IAuthRepository _authRepository,
        IAgencyRepository _agencyRepository,
        ApplicationDbContext _dbContext,
        IEmailJobService _emailJobService
    ) : IRegistrationStrategy
    {
        public async Task<ApplicationUser> ExecuteAsync(RegisterRequest request)
        {
            var agencyRequest = (AgencyOwnerRegisterRequest)request;

            await using IDbContextTransaction transaction =
                await _dbContext.Database.BeginTransactionAsync();

            try
            {
                var user = BuildUser(agencyRequest);

                var result = await _authRepository.CreateUserAsync(user, agencyRequest.Password);
                if (!result.Succeeded)
                    throw new RegistrationFailedException("Registration failed.");

                var roleResult = await _authRepository.AddToRoleAsync(user, Roles.AgencyOwner);
                if (!roleResult.Succeeded)
                    throw new RegistrationFailedException("User created but assigning role failed.");

                await BuildAgencyAsync(user, agencyRequest);

                await transaction.CommitAsync();
                return user;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
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
