using Booking.Data;
using Booking.Enums;
using Booking.Interfaces.Repositories;
using Booking.Models;
using Microsoft.EntityFrameworkCore;

namespace Booking.Repositories
{
    public class PaymentLogRepository(
        ApplicationDbContext _context,
        ILogger<PaymentLogRepository> _logger) : IPaymentLogRepository
    {
        public async Task<PaymentLog> CreateAsync(PaymentLog paymentLog)
        {
            _logger.LogDebug("Creating payment log for reservation {ReservationId}", paymentLog.ReservationId);
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

        public async Task<HotelPaymentSummary> GetHotelSummaryAsync(int hotelId)
        {
            var rows = await _context.PaymentLogs
                .Where(p => p.To == hotelId || p.From == hotelId)
                .Select(p => new { IsIncoming = p.To == hotelId, p.Amount })
                .GroupBy(x => x.IsIncoming)
                .Select(g => new { IsIncoming = g.Key, Count = g.Count(), Total = g.Sum(x => x.Amount) })
                .ToListAsync();

            var inc = rows.FirstOrDefault(r => r.IsIncoming);
            var outg = rows.FirstOrDefault(r => !r.IsIncoming);
            return new(inc?.Total ?? 0, outg?.Total ?? 0, inc?.Count ?? 0, outg?.Count ?? 0);
        }

        public async Task<PaymentLog?> GetByIdAsync(int paymentLogId)
            => await _context.PaymentLogs
                .Include(p => p.Reservation)
                .FirstOrDefaultAsync(p => p.Id == paymentLogId);


        public async Task<decimal> GetTotalIncomingByAgencyAsync(int agencyId)
            => await _context.PaymentLogs
                .Join(_context.Hotels,
                      pl => pl.To,
                      h => h.Id,
                      (pl, h) => new { pl.Amount, h.AgencyId })
                .Where(x => x.AgencyId == agencyId)
                .SumAsync(x => x.Amount);

        public async Task<IEnumerable<MonthlyRevenue>> GetMonthlyIncomingByAgencyAsync(
            int agencyId, DateTime from, DateTime to)
        {
            var rows = await _context.PaymentLogs
                .Join(_context.Hotels,
                      pl => pl.To,
                      h => h.Id,
                      (pl, h) => new { pl.Amount, pl.CreatedAt, h.AgencyId })
                .Where(x => x.AgencyId == agencyId && x.CreatedAt >= from && x.CreatedAt < to)
                .GroupBy(x => new { x.CreatedAt.Year, x.CreatedAt.Month })
                .Select(g => new { g.Key.Year, g.Key.Month, Revenue = g.Sum(x => x.Amount) })
                .ToListAsync();

            return rows.Select(r => new MonthlyRevenue(r.Year, r.Month, r.Revenue));
        }

        public async Task<IEnumerable<MonthlyRevenue>> GetMonthlyOutgoingByAgencyAsync(
            int agencyId, DateTime from, DateTime to)
        {
            var rows = await _context.PaymentLogs
                .Join(_context.Hotels,
                      pl => pl.From,
                      h => h.Id,
                      (pl, h) => new { pl.Amount, pl.CreatedAt, h.AgencyId })
                .Where(x => x.AgencyId == agencyId && x.CreatedAt >= from && x.CreatedAt < to)
                .GroupBy(x => new { x.CreatedAt.Year, x.CreatedAt.Month })
                .Select(g => new { g.Key.Year, g.Key.Month, Revenue = g.Sum(x => x.Amount) })
                .ToListAsync();

            return rows.Select(r => new MonthlyRevenue(r.Year, r.Month, r.Revenue));
        }

        public async Task<IEnumerable<HotelRevenue>> GetRevenuePerHotelByAgencyAsync(int agencyId)
        {
            var rows = await _context.Hotels
                .Where(h => h.AgencyId == agencyId)
                .Select(h => new
                {
                    h.Id,
                    h.Name,
                    Revenue = _context.PaymentLogs
                        .Where(pl => pl.To == h.Id)
                        .Sum(pl => (decimal?)pl.Amount) ?? 0m
                })
                .OrderByDescending(x => x.Revenue)
                .ToListAsync();

            return rows.Select(r => new HotelRevenue(r.Id, r.Name, r.Revenue));
        }

        public async Task<IEnumerable<MonthlyRevenue>> GetMonthlyIncomingByHotelAsync(
            int hotelId, DateTime from, DateTime to)
        {
            var rows = await _context.PaymentLogs
                .Where(pl => pl.To == hotelId && pl.CreatedAt >= from && pl.CreatedAt < to)
                .GroupBy(pl => new { pl.CreatedAt.Year, pl.CreatedAt.Month })
                .Select(g => new { g.Key.Year, g.Key.Month, Revenue = g.Sum(x => x.Amount) })
                .ToListAsync();
            return rows.Select(r => new MonthlyRevenue(r.Year, r.Month, r.Revenue));
        }

        public async Task<IEnumerable<MonthlyRevenue>> GetMonthlyOutgoingByHotelAsync(
            int hotelId, DateTime from, DateTime to)
        {
            var rows = await _context.PaymentLogs
                .Where(pl => pl.From == hotelId && pl.CreatedAt >= from && pl.CreatedAt < to)
                .GroupBy(pl => new { pl.CreatedAt.Year, pl.CreatedAt.Month })
                .Select(g => new { g.Key.Year, g.Key.Month, Revenue = g.Sum(x => x.Amount) })
                .ToListAsync();
            return rows.Select(r => new MonthlyRevenue(r.Year, r.Month, r.Revenue));
        }

        public async Task<IEnumerable<PaymentTypeRevenue>> GetRevenueByTypeByHotelAsync(int hotelId)
        {
            var rows = await _context.PaymentLogs
                .Where(pl => pl.To == hotelId && pl.Type != PaymentType.Refund)
                .GroupBy(pl => pl.Type)
                .Select(g => new { Type = g.Key, Revenue = g.Sum(x => x.Amount) })
                .ToListAsync();
            return rows.Select(r => new PaymentTypeRevenue(r.Type, r.Revenue));
        }

        public async Task<IEnumerable<MonthlyNet>> GetMonthlyNetByHotelAsync(int hotelId)
        {
            var rows = await _context.PaymentLogs
                .Where(pl => pl.To == hotelId || pl.From == hotelId)
                .GroupBy(pl => new { pl.CreatedAt.Year, pl.CreatedAt.Month })
                .Select(g => new
                {
                    g.Key.Year,
                    g.Key.Month,
                    Net = g.Sum(x => x.To == hotelId ? x.Amount : 0m)
                       - g.Sum(x => x.From == hotelId ? x.Amount : 0m)
                })
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
                .ToListAsync();
            return rows.Select(r => new MonthlyNet(r.Year, r.Month, r.Net));
        }

        public async Task<RefundImpactData> GetRefundImpactByHotelAsync(int hotelId)
        {
            var baseQuery = _context.PaymentLogs
                .Where(pl => pl.To == hotelId || pl.From == hotelId);

            var paidRevenue = await baseQuery
                .Where(pl => pl.To == hotelId && pl.Type == PaymentType.Booking)
                .SumAsync(pl => (decimal?)pl.Amount) ?? 0m;

            var refundAmount = await baseQuery
                .Where(pl => pl.From == hotelId && pl.Type == PaymentType.Refund)
                .SumAsync(pl => (decimal?)pl.Amount) ?? 0m;

            var cancellationLoss = await baseQuery
                .Where(pl => pl.From == hotelId && pl.Type == PaymentType.Cancellation)
                .SumAsync(pl => (decimal?)pl.Amount) ?? 0m;

            return new RefundImpactData(paidRevenue, refundAmount, cancellationLoss);
        }

        public async Task<PaymentLog> UpdateAsync(PaymentLog paymentLog)
        {
            _context.PaymentLogs.Update(paymentLog);
            await _context.SaveChangesAsync();
            return paymentLog;
        }

        public async Task DeleteAsync(PaymentLog paymentLog)
        {
            _context.PaymentLogs.Remove(paymentLog);
            await _context.SaveChangesAsync();
        }

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
                true => _context.PaymentLogs.Where(p => p.To == hotelId),
                false => _context.PaymentLogs.Where(p => p.From == hotelId),
                null => _context.PaymentLogs.Where(p => p.To == hotelId || p.From == hotelId)
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
