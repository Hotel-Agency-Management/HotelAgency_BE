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

        public async Task<IEnumerable<PaymentLog>> GetAllAsync(
            PaymentType? type, DateTime? dateFrom, DateTime? dateTo, int pageNumber, int pageSize)
            => await BuildQuery(null, type, dateFrom, dateTo)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

        public async Task<int> CountAllAsync(PaymentType? type, DateTime? dateFrom, DateTime? dateTo)
            => await BuildQuery(null, type, dateFrom, dateTo).CountAsync();

        public async Task<IEnumerable<PaymentLog>> GetByHotelIdAsync(
            int hotelId, PaymentType? type, DateTime? dateFrom, DateTime? dateTo, int pageNumber, int pageSize)
            => await BuildQuery(hotelId, type, dateFrom, dateTo)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

        public async Task<int> CountByHotelIdAsync(
            int hotelId, PaymentType? type, DateTime? dateFrom, DateTime? dateTo)
            => await BuildQuery(hotelId, type, dateFrom, dateTo).CountAsync();

        public async Task<IEnumerable<PaymentLog>> GetIncomingByHotelIdAsync(
            int hotelId, PaymentType? type, DateTime? dateFrom, DateTime? dateTo, int pageNumber, int pageSize)
            => await BuildDirectionalQuery(hotelId, incoming: true, type, dateFrom, dateTo)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

        public async Task<int> CountIncomingByHotelIdAsync(
            int hotelId, PaymentType? type, DateTime? dateFrom, DateTime? dateTo)
            => await BuildDirectionalQuery(hotelId, incoming: true, type, dateFrom, dateTo).CountAsync();

        public async Task<decimal> SumIncomingByHotelIdAsync(
            int hotelId, PaymentType? type, DateTime? dateFrom, DateTime? dateTo)
            => await BuildDirectionalQuery(hotelId, incoming: true, type, dateFrom, dateTo)
                .SumAsync(p => p.Amount);

        public async Task<IEnumerable<PaymentLog>> GetOutgoingByHotelIdAsync(
            int hotelId, PaymentType? type, DateTime? dateFrom, DateTime? dateTo, int pageNumber, int pageSize)
            => await BuildDirectionalQuery(hotelId, incoming: false, type, dateFrom, dateTo)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

        public async Task<int> CountOutgoingByHotelIdAsync(
            int hotelId, PaymentType? type, DateTime? dateFrom, DateTime? dateTo)
            => await BuildDirectionalQuery(hotelId, incoming: false, type, dateFrom, dateTo).CountAsync();

        public async Task<decimal> SumOutgoingByHotelIdAsync(
            int hotelId, PaymentType? type, DateTime? dateFrom, DateTime? dateTo)
            => await BuildDirectionalQuery(hotelId, incoming: false, type, dateFrom, dateTo)
                .SumAsync(p => p.Amount);

        private IQueryable<PaymentLog> BuildQuery(
            int? hotelId, PaymentType? type, DateTime? dateFrom, DateTime? dateTo)
        {
            var query = _context.PaymentLogs.AsQueryable();

            if (hotelId.HasValue)
                query = query.Where(p => p.To == hotelId.Value);

            if (type.HasValue)
                query = query.Where(p => p.Type == type.Value);

            if (dateFrom.HasValue)
                query = query.Where(p => p.CreatedAt >= dateFrom.Value);

            if (dateTo.HasValue)
                query = query.Where(p => p.CreatedAt <= dateTo.Value);

            return query.OrderByDescending(p => p.CreatedAt);
        }

        private IQueryable<PaymentLog> BuildDirectionalQuery(
            int hotelId, bool incoming, PaymentType? type, DateTime? dateFrom, DateTime? dateTo)
        {
            var query = incoming
                ? _context.PaymentLogs.Where(p => p.To == hotelId)
                : _context.PaymentLogs.Where(p => p.From == hotelId);

            if (type.HasValue)
                query = query.Where(p => p.Type == type.Value);

            if (dateFrom.HasValue)
                query = query.Where(p => p.CreatedAt >= dateFrom.Value);

            if (dateTo.HasValue)
                query = query.Where(p => p.CreatedAt <= dateTo.Value);

            return query.OrderByDescending(p => p.CreatedAt);
        }
    }
}
