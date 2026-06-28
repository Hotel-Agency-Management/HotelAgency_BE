using Booking.Data;
using Booking.Enums;
using Booking.Interfaces.Repositories;
using Booking.Models;
using Microsoft.EntityFrameworkCore;

namespace Booking.Repositories
{
    public class HousekeepingTicketRepository(
        ApplicationDbContext _context,
        ILogger<HousekeepingTicketRepository> _logger)
        : IHousekeepingTicketRepository
    {
        public async Task<HousekeepingTicket> CreateAsync(HousekeepingTicket ticket)
        {
            _logger.LogDebug("Creating ticket for hotel {HotelId}", ticket.HotelId);
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

        public async Task<(int Total, int Done)> GetCompletionCountsByHotelIdAsync(int hotelId)
        {
            var query = _context.HousekeepingTickets.Where(t => t.HotelId == hotelId);
            var total = await query.CountAsync();
            var done  = await query.CountAsync(t => t.Status == TicketStatus.Done);
            return (total, done);
        }

        public async Task<IEnumerable<(TicketStatus Status, int Count)>> GetStatusDistributionByHotelIdAsync(int hotelId)
        {
            var rows = await _context.HousekeepingTickets
                .Where(t => t.HotelId == hotelId)
                .GroupBy(t => t.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToListAsync();

            return rows.Select(r => (r.Status, r.Count));
        }

        public async Task<(int Total, int Done, int Overdue, int HighPriority)> GetKpiCountsByHotelIdAsync(int hotelId)
        {
            var now = DateTime.UtcNow;
            var query = _context.HousekeepingTickets.Where(t => t.HotelId == hotelId);

            var total        = await query.CountAsync();
            var done         = await query.CountAsync(t => t.Status == TicketStatus.Done);
            var overdue      = await query.CountAsync(t => t.Deadline < now && t.Status != TicketStatus.Done);
            var highPriority = await query.CountAsync(
                t => t.Priority == TicketPriority.High || t.Priority == TicketPriority.Urgent);

            return (total, done, overdue, highPriority);
        }

        public async Task<IEnumerable<DateTime>> GetCreationDatesInRangeAsync(int hotelId, DateTime from, DateTime to)
        {
            return await _context.HousekeepingTickets
                .Where(t => t.HotelId == hotelId && t.CreatedAt >= from && t.CreatedAt <= to)
                .Select(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<(TicketType Type, int Count)>> GetTypeDistributionByHotelIdAsync(int hotelId)
        {
            var rows = await _context.HousekeepingTickets
                .Where(t => t.HotelId == hotelId)
                .GroupBy(t => t.Type)
                .Select(g => new { Type = g.Key, Count = g.Count() })
                .ToListAsync();

            return rows.Select(r => (r.Type, r.Count));
        }

        public async Task<IEnumerable<(TicketPriority Priority, int Count)>> GetPriorityDistributionByHotelIdAsync(int hotelId)
        {
            var rows = await _context.HousekeepingTickets
                .Where(t => t.HotelId == hotelId)
                .GroupBy(t => t.Priority)
                .Select(g => new { Priority = g.Key, Count = g.Count() })
                .ToListAsync();

            return rows.Select(r => (r.Priority, r.Count));
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
