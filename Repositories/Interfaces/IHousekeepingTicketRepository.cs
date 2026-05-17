using Booking.Enums;
using Booking.Models;

namespace Booking.Interfaces.Repositories
{
    public interface IHousekeepingTicketRepository
    {
        Task<HousekeepingTicket> CreateAsync(HousekeepingTicket ticket);
        Task<HousekeepingTicket?> GetByIdAsync(int ticketId);
        Task<HousekeepingTicket?> GetByIdAndHotelIdAsync(int ticketId, int hotelId);
        Task<IEnumerable<HousekeepingTicket>> GetByHotelIdAsync(
            int hotelId,
            TicketStatus? status,
            TicketType? type,
            TicketPriority? priority,
            int? assignedToId,
            int pageNumber,
            int pageSize);
        Task<int> CountByHotelIdAsync(
            int hotelId,
            TicketStatus? status,
            TicketType? type,
            TicketPriority? priority,
            int? assignedToId);
        Task<IEnumerable<HousekeepingTicket>> GetAllByHotelIdAsync(int hotelId);
        Task<HousekeepingTicket> UpdateAsync(HousekeepingTicket ticket);
        Task DeleteAsync(HousekeepingTicket ticket);
    }
}
