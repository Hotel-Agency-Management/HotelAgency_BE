using Booking.Models;

namespace Booking.Interfaces.Services
{
    public interface IJwtService
    {
        string GenerateToken(ApplicationUser user, string role);
    }
}
