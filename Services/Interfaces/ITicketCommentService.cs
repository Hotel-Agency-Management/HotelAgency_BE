using Booking.DTO;

namespace Booking.Interfaces.Services
{
    public interface ITicketCommentService
    {
        Task<TicketCommentResponse> AddCommentAsync(int ticketId, int authorUserId, CreateTicketCommentRequest request);
        Task<IEnumerable<TicketCommentResponse>> GetCommentsByTicketAsync(int ticketId);
        Task<TicketCommentResponse> UpdateCommentAsync(int ticketId, int commentId, int requestingUserId, UpdateTicketCommentRequest request);
        Task DeleteCommentAsync(int ticketId, int commentId, int requestingUserId, bool isAdmin = false);
    }
}
