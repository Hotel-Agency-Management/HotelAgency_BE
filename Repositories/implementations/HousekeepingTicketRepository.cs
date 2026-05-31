using Booking.Data;
using Booking.Enums;
using Booking.Interfaces.Repositories;
using Booking.Models;
using Microsoft.EntityFrameworkCore;

namespace Booking.Repositories
{
    public class HousekeepingTicketRepository(ApplicationDbContext _context)
        : IHousekeepingTicketRepository
    {
        public async Task<HousekeepingTicket> CreateAsync(HousekeepingTicket ticket)
        {
            await _context.HousekeepingTickets.AddAsync(ticket);
            await _context.SaveChangesAsync();
            return await GetByIdAsync(ticket.Id) ?? ticket;
        }

        public async Task<HousekeepingTicket?> GetByIdAsync(int ticketId)
            => await _context.HousekeepingTickets
                .Include(t => t.AssignedTo)
                .Include(t => t.CreatedBy)
                .Include(t => t.Room)
                .Include(t => t.Facility)
                .Include(t => t.Comments)
                    .ThenInclude(c => c.Author)
                .FirstOrDefaultAsync(t => t.Id == ticketId);

        public async Task<HousekeepingTicket?> GetByIdAndHotelIdAsync(int ticketId, int hotelId)
            => await _context.HousekeepingTickets
                .Include(t => t.AssignedTo)
                .Include(t => t.CreatedBy)
                .Include(t => t.Room)
                .Include(t => t.Facility)
                .Include(t => t.Comments)
                    .ThenInclude(c => c.Author)
                .FirstOrDefaultAsync(t => t.Id == ticketId && t.HotelId == hotelId);

        public async Task<IEnumerable<HousekeepingTicket>> GetByHotelIdAsync(
            int hotelId,
            TicketStatus? status,
            TicketType? type,
            TicketPriority? priority,
            int? assignedToId,
            int pageNumber,
            int pageSize,
            int? visibleToUserId = null)
            => await BuildQuery(hotelId, status, type, priority, assignedToId, visibleToUserId)
                .Include(t => t.AssignedTo)
                .Include(t => t.Room)
                .Include(t => t.Facility)
                .OrderByDescending(t => t.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

        public async Task<int> CountByHotelIdAsync(
            int hotelId,
            TicketStatus? status,
            TicketType? type,
            TicketPriority? priority,
            int? assignedToId,
            int? visibleToUserId = null)
            => await BuildQuery(hotelId, status, type, priority, assignedToId, visibleToUserId).CountAsync();

        public async Task<IEnumerable<HousekeepingTicket>> GetAllByHotelIdAsync(int hotelId,
            int? visibleToUserId = null)
        {
            var q = _context.HousekeepingTickets.Where(t => t.HotelId == hotelId);

            if (visibleToUserId.HasValue)
                q = q.Where(t => t.CreatedById == visibleToUserId.Value
                               || t.AssignedToId == visibleToUserId.Value);

            return await q
                .Include(t => t.AssignedTo)
                .Include(t => t.Room)
                .Include(t => t.Facility)
                .OrderBy(t => t.CreatedAt)
                .ThenByDescending(t => t.Priority)
                .ToListAsync();
        }

        public async Task<HousekeepingTicket> UpdateAsync(HousekeepingTicket ticket)
        {
            _context.HousekeepingTickets.Update(ticket);
            await _context.SaveChangesAsync();
            return await GetByIdAsync(ticket.Id) ?? ticket;
        }

        public async Task DeleteAsync(HousekeepingTicket ticket)
        {
            _context.HousekeepingTickets.Remove(ticket);
            await _context.SaveChangesAsync();
        }

        private IQueryable<HousekeepingTicket> BuildQuery(
            int hotelId,
            TicketStatus? status,
            TicketType? type,
            TicketPriority? priority,
            int? assignedToId,
            int? visibleToUserId = null)
        {
            var q = _context.HousekeepingTickets.Where(t => t.HotelId == hotelId);

            if (visibleToUserId.HasValue)
                q = q.Where(t => t.CreatedById == visibleToUserId.Value
                               || t.AssignedToId == visibleToUserId.Value);

            if (status.HasValue)
                q = q.Where(t => t.Status == status.Value);

            if (type.HasValue)
                q = q.Where(t => t.Type == type.Value);

            if (priority.HasValue)
                q = q.Where(t => t.Priority == priority.Value);

            if (assignedToId.HasValue)
                q = q.Where(t => t.AssignedToId == assignedToId.Value);

            return q;
        }
    }
}
