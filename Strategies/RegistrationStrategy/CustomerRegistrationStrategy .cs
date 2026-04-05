
using Booking.Interfaces.Repositories;
using Booking.Models;
using Booking.DTO;
using Booking.Exceptions;
using Booking.Constants;
using Booking.Enums;
namespace Booking.Strategies
{
    public class CustomerRegistrationStrategy (IAuthRepository _authRepository): IRegistrationStrategy
    {
        public async Task <ApplicationUser> ExecuteAsync(RegisterRequest request)
        {
            var user = BuildUser(request);

            var result = await _authRepository.CreateUserAsync(user, request.Password);
            if (!result.Succeeded)
                throw new RegistrationFailedException("Registration failed.");

            var roleResult = await _authRepository.AddToRoleAsync(user, Roles.Customer);
            if (!roleResult.Succeeded)
                throw new RegistrationFailedException("User created but assigning role failed.");
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
    }

}