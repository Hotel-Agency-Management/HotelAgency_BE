using Booking.Data;
using Booking.Enums;
using Booking.Interfaces.Repositories;
using Booking.Models;
using Microsoft.EntityFrameworkCore;

namespace Booking.Repositories
{
    public class PaymentLogRepository(ApplicationDbContext _context) : IPaymentLogRepository
    {
        public async Task<PaymentLog> CreateAsync(PaymentLog paymentLog)
        {
            await _context.PaymentLogs.AddAsync(paymentLog);
            await _context.SaveChangesAsync();
            return paymentLog;
        }

        public async Task<IEnumerable<PaymentLog>> GetAllPagedAsync(
            PaymentType? type, DateTime? dateFrom, DateTime? dateTo, bool ascending, int pageNumber, int pageSize)
            => await BuildBaseQuery(type, dateFrom, dateTo, ascending)
                .Include(p => p.Reservation)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

        public async Task<int> CountAllAsync(PaymentType? type, DateTime? dateFrom, DateTime? dateTo)
            => await BuildBaseQuery(type, dateFrom, dateTo, ascending: false).CountAsync();

        public async Task<IEnumerable<PaymentLog>> GetHotelLogsAsync(
            int hotelId, bool? incoming, PaymentType? type, DateTime? dateFrom, DateTime? dateTo,
            bool ascending, int pageNumber, int pageSize)
            => await BuildHotelQuery(hotelId, incoming, type, dateFrom, dateTo, ascending)
                .Include(p => p.Reservation)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

        public async Task<int> CountHotelLogsAsync(
            int hotelId, bool? incoming, PaymentType? type, DateTime? dateFrom, DateTime? dateTo)
            => await BuildHotelQuery(hotelId, incoming, type, dateFrom, dateTo, ascending: false).CountAsync();

        public async Task<decimal> SumHotelIncomingAsync(int hotelId)
            => await _context.PaymentLogs.Where(p => p.To == hotelId).SumAsync(p => p.Amount);

        public async Task<decimal> SumHotelOutgoingAsync(int hotelId)
            => await _context.PaymentLogs.Where(p => p.From == hotelId).SumAsync(p => p.Amount);

        public async Task<int> CountHotelIncomingAsync(int hotelId)
            => await _context.PaymentLogs.CountAsync(p => p.To == hotelId);

        public async Task<int> CountHotelOutgoingAsync(int hotelId)
            => await _context.PaymentLogs.CountAsync(p => p.From == hotelId);

        public async Task<PaymentLog?> GetByIdAsync(int paymentLogId)
            => await _context.PaymentLogs
                .Include(p => p.Reservation)
                .FirstOrDefaultAsync(p => p.Id == paymentLogId);

        private IQueryable<PaymentLog> BuildBaseQuery(
            PaymentType? type, DateTime? dateFrom, DateTime? dateTo, bool ascending)
        {
            var query = _context.PaymentLogs.AsQueryable();

            if (type.HasValue)
                query = query.Where(p => p.Type == type.Value);
            if (dateFrom.HasValue)
                query = query.Where(p => p.CreatedAt >= dateFrom.Value);
            if (dateTo.HasValue)
                query = query.Where(p => p.CreatedAt <= dateTo.Value);

            return ascending
                ? query.OrderBy(p => p.CreatedAt)
                : query.OrderByDescending(p => p.CreatedAt);
        }

        private IQueryable<PaymentLog> BuildHotelQuery(
            int hotelId, bool? incoming, PaymentType? type, DateTime? dateFrom, DateTime? dateTo, bool ascending)
        {
            var query = incoming switch
            {
                true  => _context.PaymentLogs.Where(p => p.To == hotelId),
                false => _context.PaymentLogs.Where(p => p.From == hotelId),
                null  => _context.PaymentLogs.Where(p => p.To == hotelId || p.From == hotelId)
            };

            if (type.HasValue)
                query = query.Where(p => p.Type == type.Value);
            if (dateFrom.HasValue)
                query = query.Where(p => p.CreatedAt >= dateFrom.Value);
            if (dateTo.HasValue)
                query = query.Where(p => p.CreatedAt <= dateTo.Value);

            return ascending
                ? query.OrderBy(p => p.CreatedAt)
                : query.OrderByDescending(p => p.CreatedAt);
        }
    }
}
