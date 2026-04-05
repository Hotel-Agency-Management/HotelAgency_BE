using Booking.DTO;
using Booking.Models;
namespace Booking.Strategies
{
    public interface IRegistrationStrategy
    {
        Task <ApplicationUser> ExecuteAsync(RegisterRequest request);
    }
}