using Booking.DTO;
using Booking.Enums;

namespace Booking.Interfaces.Services
{
    public interface IHousekeepingTicketService
    {
        Task<TicketDetailResponse> CreateTicketAsync(int hotelId, int createdByUserId, CreateTicketRequest request);
        Task<TicketDetailResponse> GetTicketByIdAsync(int hotelId, int ticketId);
        Task<PaginatedResponse<TicketListItemResponse>> GetTicketsByHotelAsync(int hotelId, TicketListRequest request);
        Task<TicketBoardResponse> GetBoardAsync(int hotelId);
        Task<TicketDetailResponse> UpdateTicketAsync(int hotelId, int ticketId, UpdateTicketRequest request);
        Task<TicketDetailResponse> UpdateTicketStatusAsync(int hotelId, int ticketId, UpdateTicketStatusRequest request);
        Task DeleteTicketAsync(int hotelId, int ticketId);
    }
}
