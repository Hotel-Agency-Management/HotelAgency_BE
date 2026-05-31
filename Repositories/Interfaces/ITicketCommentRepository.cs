using Booking.Models;

namespace Booking.Interfaces.Repositories
{
    public interface ITicketCommentRepository
    {
        Task<TicketComment?> GetByIdAsync(int commentId);
        Task<IEnumerable<TicketComment>> GetByTicketIdAsync(int ticketId, int pageNumber, int pageSize);
        Task<int> CountByTicketIdAsync(int ticketId);
        Task<TicketComment> AddAsync(TicketComment comment);
        Task<TicketComment> UpdateAsync(TicketComment comment);
        Task DeleteAsync(TicketComment comment);
    }
}
