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
            int pageSize,
            int? visibleToUserId = null);
        Task<int> CountByHotelIdAsync(
            int hotelId,
            TicketStatus? status,
            TicketType? type,
            TicketPriority? priority,
            int? assignedToId,
            int? visibleToUserId = null);
        Task<IEnumerable<HousekeepingTicket>> GetAllByHotelIdAsync(int hotelId,
            int? visibleToUserId = null);
        Task<HousekeepingTicket> UpdateAsync(HousekeepingTicket ticket);
        Task DeleteAsync(HousekeepingTicket ticket);
        Task<(int Total, int Done)> GetCompletionCountsByHotelIdAsync(int hotelId);
        Task<IEnumerable<(TicketStatus Status, int Count)>> GetStatusDistributionByHotelIdAsync(int hotelId);
        Task<(int Total, int Done, int Overdue, int HighPriority)> GetKpiCountsByHotelIdAsync(int hotelId);
        Task<IEnumerable<DateTime>> GetCreationDatesInRangeAsync(int hotelId, DateTime from, DateTime to);
        Task<IEnumerable<(TicketType Type, int Count)>> GetTypeDistributionByHotelIdAsync(int hotelId);
        Task<IEnumerable<(TicketPriority Priority, int Count)>> GetPriorityDistributionByHotelIdAsync(int hotelId);
    }
}
