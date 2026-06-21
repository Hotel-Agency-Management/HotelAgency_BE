using Booking.Data;
using Booking.Interfaces.Repositories;
using Booking.Models;
using Microsoft.EntityFrameworkCore;

namespace Booking.Repositories
{
    public class TicketCommentRepository(
        ApplicationDbContext _context,
        ILogger<TicketCommentRepository> _logger)
        : ITicketCommentRepository
    {
        public async Task<TicketComment?> GetByIdAsync(int commentId)
            => await _context.TicketComments
                .Include(c => c.Author)
                .FirstOrDefaultAsync(c => c.Id == commentId);

        public async Task<IEnumerable<TicketComment>> GetByTicketIdAsync(int ticketId, int pageNumber, int pageSize)
            => await _context.TicketComments
                .Include(c => c.Author)
                .Where(c => c.TicketId == ticketId)
                .OrderBy(c => c.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

        public async Task<int> CountByTicketIdAsync(int ticketId)
            => await _context.TicketComments
                .CountAsync(c => c.TicketId == ticketId);

        public async Task<TicketComment> AddAsync(TicketComment comment)
        {
            _logger.LogDebug("Adding comment to ticket {TicketId}", comment.TicketId);
            await _context.TicketComments.AddAsync(comment);
            await _context.SaveChangesAsync();
            return await GetByIdAsync(comment.Id) ?? comment;
        }

        public async Task<TicketComment> UpdateAsync(TicketComment comment)
        {
            _context.TicketComments.Update(comment);
            await _context.SaveChangesAsync();
            return await GetByIdAsync(comment.Id) ?? comment;
        }

        public async Task DeleteAsync(TicketComment comment)
        {
            _context.TicketComments.Remove(comment);
            await _context.SaveChangesAsync();
        }
    }
}
