using Booking.Interfaces.Repositories;
using Booking.Models;
using Booking.DTO;
using Booking.Exceptions;
using Booking.Constants;
using Booking.Enums;

namespace Booking.Strategies
{
    public class AgencyOwnerRegistrationStrategy(
        IAuthRepository _authRepository,
        IAgencyRepository _agencyRepository
    ) : IRegistrationStrategy
    {
        public async Task<ApplicationUser> ExecuteAsync(RegisterRequest request)
        {
            var user = BuildUser(request);

            var result = await _authRepository.CreateUserAsync(user, request.Password);
            if (!result.Succeeded)
                throw new RegistrationFailedException("Registration failed.");

            var roleResult = await _authRepository.AddToRoleAsync(user, Roles.AgencyOwner);
            if (!roleResult.Succeeded)
                throw new RegistrationFailedException("User created but assigning role failed.");

            await BuildAgencyAsync(user, request);


            return user;
        }

        private static ApplicationUser BuildUser(RegisterRequest request) => new()
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

        private async Task BuildAgencyAsync(ApplicationUser user, RegisterRequest request)
        {
            var exists = await _agencyRepository.ExistsByNameAsync(request.AgencyName!);
            if (exists)
                throw new AgencyAlreadyExistsException(request.AgencyName!);

            var agency = new Agency
            {
                AgencyName = request.AgencyName!.Trim(),
                Country = request.Country!.Trim(),
                City = request.City!.Trim(),
                Phone = request.Phone!.Trim(),
                OwnerId = user.Id,
                Status = AgencyStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            await _agencyRepository.AddAsync(agency);

            user.AgencyId = agency.Id;
            var updated = await _authRepository.UpdateUserAsync(user);
            if (!updated)
                throw new RegistrationFailedException("Failed to link agency to user.");
        }
    }

}
