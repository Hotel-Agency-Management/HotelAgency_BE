using Booking.Constants;
using Booking.DTO;
using Booking.Enums;
using Booking.Exceptions;
using Booking.Interfaces.Repositories;
using Booking.Interfaces.Services;
using Booking.Models;
using Microsoft.AspNetCore.Identity;

namespace Booking.Services
{
    public class PaymentLogService(
        IPaymentLogRepository _paymentLogRepository,
        UserManager<ApplicationUser> _userManager,
        IHotelRepository _hotelRepository,
        ISystemLogService _logService,
        ILogger<PaymentLogService> _logger) : IPaymentLogService
    {
        public async Task<PaginatedResponse<PaymentLogItemResponse>> GetAllAsync(PaymentLogListRequest request)
        {
            bool ascending = IsAscending(request.SortOrder);

            var items = (await _paymentLogRepository.GetAllPagedAsync(
                request.Type, request.DateFrom, request.DateTo, ascending,
                request.PageNumber, request.PageSize)).ToList();
            var total = await _paymentLogRepository.CountAllAsync(
                request.Type, request.DateFrom, request.DateTo);


            var allIds = items.SelectMany(p => new[] { p.From, p.To }).Distinct().ToList();
            var nameMap = new Dictionary<int, string>();
            var hotelIdSet = new HashSet<int>();

            foreach (var id in allIds)
            {
                var user = await _userManager.FindByIdAsync(id.ToString());
                if (user is not null)
                {
                    nameMap[id] = $"{user.FirstName} {user.LastName}";
                    continue;
                }
                var hotel = await _hotelRepository.GetByIdAsync(id);
                if (hotel is not null)
                {
                    nameMap[id] = hotel.Name;
                    hotelIdSet.Add(id);
                }
            }

            string ResolveName(int id) =>
                nameMap.TryGetValue(id, out var name) ? name : id.ToString();

            var mapped = items.Select(p => new PaymentLogItemResponse
            {
                PaymentId = p.Id,
                ReservationReference = p.Reservation?.ReservationNumber,
                PaymentType = p.Type.ToString(),
                Reason = p.Reason,
                Amount = p.Amount,
                FromName = ResolveName(p.From),
                ToName = ResolveName(p.To),
                CreatedAt = p.CreatedAt,
            }).ToList();

            return new PaginatedResponse<PaymentLogItemResponse>
            {
                Items = mapped,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = total,
                TotalPages = (int)Math.Ceiling(total / (double)request.PageSize)
            };
        }

        public async Task<PaymentLogSummaryResponse> GetHotelLogsAsync(
            int hotelId, bool? incoming, PaymentLogListRequest request)
        {
            bool ascending = IsAscending(request.SortOrder);

            var items = (await _paymentLogRepository.GetHotelLogsAsync(hotelId, incoming, request.Type, request.DateFrom, request.DateTo, ascending, request.PageNumber, request.PageSize)).ToList();
            var total = await _paymentLogRepository.CountHotelLogsAsync(hotelId, incoming, request.Type, request.DateFrom, request.DateTo);
            var summary = await _paymentLogRepository.GetHotelSummaryAsync(hotelId);

            var hotel = await _hotelRepository.GetByIdAsync(hotelId);
            var hotelName = hotel?.Name ?? hotelId.ToString();

            var userIds = items
                .Select(p => p.To == hotelId ? p.From : p.To)
                .Distinct()
                .ToList();

            var userMap = new Dictionary<int, string>();
            foreach (var uid in userIds)
            {
                var user = await _userManager.FindByIdAsync(uid.ToString());
                userMap[uid] = user is not null ? $"{user.FirstName} {user.LastName}" : uid.ToString();
            }

            var mapped = items.Select(p =>
            {
                bool isIncoming = p.To == hotelId;
                int userId = isIncoming ? p.From : p.To;
                string userName = userMap.TryGetValue(userId, out var n) ? n : userId.ToString();

                return new PaymentLogItemResponse
                {
                    PaymentId = p.Id,
                    ReservationReference = p.Reservation?.ReservationNumber,
                    PaymentType = p.Type.ToString(),
                    Reason = p.Reason,
                    Amount = p.Amount,
                    FromName = isIncoming ? userName : hotelName,
                    ToName = isIncoming ? hotelName : userName,
                    CreatedAt = p.CreatedAt,
                };
            }).ToList();

            var groups = mapped
                .GroupBy(p => GetWeekStart(p.CreatedAt))
                .OrderByDescending(g => g.Key)
                .Select(g => new PaymentLogGroupResponse
                {
                    WeekStart = g.Key,
                    WeekEnd = g.Key.AddDays(6),
                    Items = g.ToList()
                })
                .ToList();

            return new PaymentLogSummaryResponse
            {
                TotalIncoming = summary.TotalIncoming,
                TotalOutgoing = summary.TotalOutgoing,
                IncomingCount = summary.IncomingCount,
                OutgoingCount = summary.OutgoingCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = total,
                TotalPages = (int)Math.Ceiling(total / (double)request.PageSize),
                Groups = groups
            };
        }

        public async Task<PaymentLogDetailsResponse> GetDetailsAsync(int hotelId, int paymentLogId)
        {
            var log = await _paymentLogRepository.GetByIdAsync(paymentLogId)
                ?? throw new PaymentLogNotFoundException(paymentLogId);

            if (log.To != hotelId && log.From != hotelId)
                throw new PaymentLogForbiddenException(paymentLogId, hotelId);

            var hotel = await _hotelRepository.GetByIdAsync(hotelId);
            var hotelName = hotel?.Name ?? hotelId.ToString();

            bool isIncoming = log.To == hotelId;
            int userId = isIncoming ? log.From : log.To;
            var user = await _userManager.FindByIdAsync(userId.ToString());
            var userName = user is not null ? $"{user.FirstName} {user.LastName}" : userId.ToString();

            return new PaymentLogDetailsResponse
            {
                PaymentId = log.Id,
                PaymentType = log.Type.ToString(),
                Reason = log.Reason,
                Amount = log.Amount,
                CreatedAt = log.CreatedAt,
                ReservationReference = log.Reservation?.ReservationNumber,
                ReservationId = log.ReservationId,
                FromName = isIncoming ? userName : hotelName,
                ToName = isIncoming ? hotelName : userName,
                Timeline = BuildTimeline(log)
            };
        }

        public async Task<decimal> GetAgencyRevenueStatsAsync(int agencyId)
            => await _paymentLogRepository.GetTotalIncomingByAgencyAsync(agencyId);

        public async Task<IReadOnlyList<MonthlyRevenueItem>> GetAgencyMonthlyProfitAsync(int agencyId)
        {
            var now = DateTime.UtcNow;
            var from = new DateTime(now.Year, now.Month, RevenueReportConstants.FirstDayOfMonth,
               RevenueReportConstants.ResetHour, RevenueReportConstants.ResetMinute, RevenueReportConstants.ResetSecond,
               DateTimeKind.Utc).AddMonths(RevenueReportConstants.MonthsBack);

            var to = new DateTime(now.Year, now.Month, RevenueReportConstants.FirstDayOfMonth,
               RevenueReportConstants.ResetHour, RevenueReportConstants.ResetMinute, RevenueReportConstants.ResetSecond,
                DateTimeKind.Utc).AddMonths(RevenueReportConstants.MonthsForward);

            var dbRows = (await _paymentLogRepository.GetMonthlyIncomingByAgencyAsync(agencyId, from, to))
                .ToDictionary(r => (r.Year, r.Month), r => r.Revenue);

            var result = new List<MonthlyRevenueItem>(RevenueReportConstants.PeriodLengthMonths);
            for (int i = 0; i < RevenueReportConstants.PeriodLengthMonths; i++)
            {
                var month = from.AddMonths(i);
                dbRows.TryGetValue((month.Year, month.Month), out var profit);
                result.Add(new MonthlyRevenueItem
                {
                    Month = month.ToString(RevenueReportConstants.MonthFormat),
                    Year = month.Year,
                    Revenue = profit
                });
            }
            return result;
        }

        public async Task<IReadOnlyList<MonthlyRevenueItem>> GetAgencyMonthlyExpensesAsync(int agencyId)
        {
            var now = DateTime.UtcNow;
            var from = new DateTime(now.Year, now.Month, RevenueReportConstants.FirstDayOfMonth,
                RevenueReportConstants.ResetHour, RevenueReportConstants.ResetMinute, RevenueReportConstants.ResetSecond,
                DateTimeKind.Utc).AddMonths(RevenueReportConstants.MonthsBack);

            var to = new DateTime(now.Year, now.Month, RevenueReportConstants.FirstDayOfMonth,
                RevenueReportConstants.ResetHour, RevenueReportConstants.ResetMinute, RevenueReportConstants.ResetSecond,
                 DateTimeKind.Utc).AddMonths(RevenueReportConstants.MonthsForward);

            var dbRows = (await _paymentLogRepository.GetMonthlyOutgoingByAgencyAsync(agencyId, from, to))
                .ToDictionary(r => (r.Year, r.Month), r => r.Revenue);

            var result = new List<MonthlyRevenueItem>(RevenueReportConstants.PeriodLengthMonths);
            for (int i = 0; i < RevenueReportConstants.PeriodLengthMonths; i++)
            {
                var month = from.AddMonths(i);
                dbRows.TryGetValue((month.Year, month.Month), out var expenses);
                result.Add(new MonthlyRevenueItem
                {
                    Month = month.ToString(RevenueReportConstants.MonthFormat),
                    Year = month.Year,
                    Revenue = expenses
                });
            }
            return result;
        }

        public async Task<IReadOnlyList<CashFlowItem>> GetHotelCashFlowAsync(int hotelId)
        {
            var now = DateTime.UtcNow;
            var from = new DateTime(now.Year, now.Month, RevenueReportConstants.FirstDayOfMonth,
                RevenueReportConstants.ResetHour, RevenueReportConstants.ResetMinute,
                RevenueReportConstants.ResetSecond, DateTimeKind.Utc)
                .AddMonths(RevenueReportConstants.MonthsBack);
            var to = new DateTime(now.Year, now.Month, RevenueReportConstants.FirstDayOfMonth,
                RevenueReportConstants.ResetHour, RevenueReportConstants.ResetMinute,
                RevenueReportConstants.ResetSecond, DateTimeKind.Utc)
                .AddMonths(RevenueReportConstants.MonthsForward);

            var incomingMap = (await _paymentLogRepository.GetMonthlyIncomingByHotelAsync(hotelId, from, to))
                .ToDictionary(r => (r.Year, r.Month), r => r.Revenue);
            var outgoingMap = (await _paymentLogRepository.GetMonthlyOutgoingByHotelAsync(hotelId, from, to))
                .ToDictionary(r => (r.Year, r.Month), r => r.Revenue);

            var result = new List<CashFlowItem>(RevenueReportConstants.PeriodLengthMonths);
            for (int i = 0; i < RevenueReportConstants.PeriodLengthMonths; i++)
            {
                var month = from.AddMonths(i);
                incomingMap.TryGetValue((month.Year, month.Month), out var incoming);
                outgoingMap.TryGetValue((month.Year, month.Month), out var outgoing);
                result.Add(new CashFlowItem
                {
                    Month = month.ToString(RevenueReportConstants.MonthFormat),
                    Year = month.Year,
                    Incoming = incoming,
                    Outgoing = outgoing
                });
            }
            return result;
        }

        public async Task<IReadOnlyList<RevenueByTypeItem>> GetHotelRevenueByTypeAsync(int hotelId)
        {
            var dbRows = (await _paymentLogRepository.GetRevenueByTypeByHotelAsync(hotelId))
                .ToDictionary(r => r.Type, r => r.Revenue);

            return Enum.GetValues<PaymentType>()
                .Where(t => t != PaymentType.Refund)
                .Select(t =>
                {
                    dbRows.TryGetValue(t, out var revenue);
                    return new RevenueByTypeItem
                    {
                        PaymentType = t.ToString(),
                        Revenue = revenue
                    };
                })
                .ToList();
        }

        public async Task<IReadOnlyList<BalanceTrendItem>> GetHotelBalanceTrendAsync(int hotelId)
        {
            var now = DateTime.UtcNow;
            var windowStart = new DateTime(now.Year, now.Month, RevenueReportConstants.FirstDayOfMonth,
                RevenueReportConstants.ResetHour, RevenueReportConstants.ResetMinute,
                RevenueReportConstants.ResetSecond, DateTimeKind.Utc)
                .AddMonths(RevenueReportConstants.MonthsBack);

            var allRows = (await _paymentLogRepository.GetMonthlyNetByHotelAsync(hotelId)).ToList();

            decimal balance = allRows
                .Where(r => r.Year < windowStart.Year || (r.Year == windowStart.Year && r.Month < windowStart.Month))
                .Sum(r => r.Net);

            var windowMap = allRows
                .Where(r => r.Year > windowStart.Year || (r.Year == windowStart.Year && r.Month >= windowStart.Month))
                .ToDictionary(r => (r.Year, r.Month), r => r.Net);

            var result = new List<BalanceTrendItem>(RevenueReportConstants.PeriodLengthMonths);
            for (int i = 0; i < RevenueReportConstants.PeriodLengthMonths; i++)
            {
                var month = windowStart.AddMonths(i);
                windowMap.TryGetValue((month.Year, month.Month), out var netChange);
                balance += netChange;
                result.Add(new BalanceTrendItem
                {
                    Month   = month.ToString(RevenueReportConstants.MonthFormat),
                    Year    = month.Year,
                    Balance = balance
                });
            }
            return result;
        }

        public async Task<RefundImpactResponse> GetHotelRefundImpactAsync(int hotelId)
        {
            var data = await _paymentLogRepository.GetRefundImpactByHotelAsync(hotelId);
            return new RefundImpactResponse
            {
                PaidRevenue      = data.PaidRevenue,
                RefundAmount     = data.RefundAmount,
                CancellationLoss = data.CancellationLoss
            };
        }

        public async Task<RevenueGrowthResponse> GetHotelRevenueGrowthAsync(int hotelId, int month, int year)
        {
            var prevDate = new DateTime(year, month, 1).AddMonths(-1);

            var currentRevenue  = await _paymentLogRepository.GetMonthlyBookingRevenueByHotelAsync(hotelId, year, month);
            var previousRevenue = await _paymentLogRepository.GetMonthlyBookingRevenueByHotelAsync(hotelId, prevDate.Year, prevDate.Month);

            decimal growthPercentage = previousRevenue == 0
                ? 0m
                : Math.Round((currentRevenue - previousRevenue) / previousRevenue * 100, 2);
            decimal gaugeScore = Math.Min(growthPercentage, 100m);

            return new RevenueGrowthResponse
            {
                CurrentRevenue   = currentRevenue,
                PreviousRevenue  = previousRevenue,
                GrowthPercentage = growthPercentage,
                GaugeScore       = gaugeScore,
                Month            = month,
                Year             = year
            };
        }

        public async Task<IReadOnlyList<MonthlyRevenueItem>> GetAgencyRevenueTrendAsync(int agencyId)
        {
            var now = DateTime.UtcNow;
            var from = new DateTime(now.Year, now.Month, RevenueReportConstants.FirstDayOfMonth,
               RevenueReportConstants.ResetHour, RevenueReportConstants.ResetMinute, RevenueReportConstants.ResetSecond,
               DateTimeKind.Utc).AddMonths(RevenueReportConstants.MonthsBack);

            var to = new DateTime(now.Year, now.Month, RevenueReportConstants.FirstDayOfMonth,
                RevenueReportConstants.ResetHour, RevenueReportConstants.ResetMinute, RevenueReportConstants.ResetSecond,
                 DateTimeKind.Utc).AddMonths(RevenueReportConstants.MonthsForward);

            var dbRows = (await _paymentLogRepository.GetMonthlyIncomingByAgencyAsync(agencyId, from, to))
                .ToDictionary(r => (r.Year, r.Month), r => r.Revenue);

            var result = new List<MonthlyRevenueItem>(RevenueReportConstants.PeriodLengthMonths);
            for (int i = 0; i < RevenueReportConstants.PeriodLengthMonths; i++)
            {
                var month = from.AddMonths(i);
                dbRows.TryGetValue((month.Year, month.Month), out var revenue);
                result.Add(new MonthlyRevenueItem
                {
                    Month = month.ToString(RevenueReportConstants.MonthFormat),
                    Year = month.Year,
                    Revenue = revenue
                });
            }

            return result;
        }

        public async Task<IReadOnlyList<HotelRevenueItem>> GetAgencyRevenuePerHotelAsync(int agencyId)
        {
            var rows = await _paymentLogRepository.GetRevenuePerHotelByAgencyAsync(agencyId);
            return rows.Select(r => new HotelRevenueItem
            {
                HotelId = r.HotelId,
                HotelName = r.HotelName,
                Revenue = r.Revenue
            }).ToList();
        }

        public async Task<PaymentLogDetailsResponse> CreateAsync(int hotelId, CreatePaymentLogRequest request)
        {
            var log = await _paymentLogRepository.CreateAsync(new PaymentLog
            {
                ReservationId = request.ReservationId,
                Amount = request.Amount,
                Type = request.Type,
                Reason = ResolveReason(request.Type, request.Reason),
                From = request.From,
                To = request.To
            });

            var saved = await _paymentLogRepository.GetByIdAsync(log.Id);
            _logger.LogInformation("Payment log created for hotel {HotelId}, amount {Amount}, type {Type}", hotelId, request.Amount, request.Type);
            return await MapToDetailsAsync(hotelId, saved!);
        }

        public async Task<PaymentLogDetailsResponse> UpdateAsync(int hotelId, int paymentLogId, UpdatePaymentLogRequest request)
        {
            var log = await _paymentLogRepository.GetByIdAsync(paymentLogId)
                ?? throw new PaymentLogNotFoundException(paymentLogId);

            if (log.To != hotelId && log.From != hotelId)
                throw new PaymentLogForbiddenException(paymentLogId, hotelId);

            if (request.Amount.HasValue) log.Amount = request.Amount.Value;
            if (request.Type.HasValue)
            {
                log.Type = request.Type.Value;
                log.Reason = ResolveReason(request.Type.Value, request.Reason);
            }
            else if (request.Reason is not null)
            {
                log.Reason = request.Reason;
            }

            await _paymentLogRepository.UpdateAsync(log);
            _logger.LogInformation("Payment log {PaymentLogId} updated for hotel {HotelId}", paymentLogId, hotelId);
            return await MapToDetailsAsync(hotelId, log);
        }

        public async Task DeleteAsync(int hotelId, int paymentLogId)
        {
            var log = await _paymentLogRepository.GetByIdAsync(paymentLogId)
                ?? throw new PaymentLogNotFoundException(paymentLogId);

            if (log.To != hotelId && log.From != hotelId)
                throw new PaymentLogForbiddenException(paymentLogId, hotelId);

            await _paymentLogRepository.DeleteAsync(log);
            
            await _logService.LogAsync(
                SystemLogActions.PaymentLogDeleted,
                SystemLogEntityTypes.PaymentLog,
                paymentLogId,
                string.Format(SystemLogMessages.PaymentLogDeleted, log.Id),
                hotelId: hotelId);
            _logger.LogInformation("Payment log {PaymentLogId} deleted for hotel {HotelId}", paymentLogId, hotelId);
        }

        private async Task<PaymentLogDetailsResponse> MapToDetailsAsync(int hotelId, PaymentLog log)
        {
            var hotel = await _hotelRepository.GetByIdAsync(hotelId);
            var hotelName = hotel?.Name ?? hotelId.ToString();

            bool isIncoming = log.To == hotelId;
            int userId = isIncoming ? log.From : log.To;
            var user = await _userManager.FindByIdAsync(userId.ToString());
            var userName = user is not null ? $"{user.FirstName} {user.LastName}" : userId.ToString();

            return new PaymentLogDetailsResponse
            {
                PaymentId = log.Id,
                PaymentType = log.Type.ToString(),
                Reason = log.Reason,
                Amount = log.Amount,
                CreatedAt = log.CreatedAt,
                ReservationReference = log.Reservation?.ReservationNumber,
                ReservationId = log.ReservationId,
                FromName = isIncoming ? userName : hotelName,
                ToName = isIncoming ? hotelName : userName,
                Timeline = BuildTimeline(log)
            };
        }

        private static string ResolveReason(PaymentType type, string? overrideReason) =>
            overrideReason ?? type switch
            {
                PaymentType.Booking => PaymentReason.Booking,
                PaymentType.Cancellation => PaymentReason.Cancellation,
                PaymentType.ReservationInsurance => PaymentReason.ReservationInsurance,
                PaymentType.YearlyInsurance => PaymentReason.YearlyInsurance,
                PaymentType.Extend => PaymentReason.Extend,
                PaymentType.Damage => PaymentReason.Damage,
                PaymentType.Refund => PaymentReason.Refund,
                _ => string.Empty
            };


        private static DateOnly GetWeekStart(DateTime dt)
        {
            var date = DateOnly.FromDateTime(dt);
            int daysFromMonday = ((int)date.DayOfWeek - (int)DayOfWeek.Monday + RevenueReportConstants.DaysInWeek) % RevenueReportConstants.DaysInWeek;
            return date.AddDays(-daysFromMonday);
        }

        private static bool IsAscending(string sortOrder) =>
            sortOrder.Equals(SortOrders.Oldest, StringComparison.OrdinalIgnoreCase);


        private static IReadOnlyCollection<PaymentTimelineItem> BuildTimeline(PaymentLog log)
        {
            var timeline = new List<PaymentTimelineItem>();
            var res = log.Reservation;

            if (res is not null)
            {
                timeline.Add(new() { Event = "Reservation Created", OccurredAt = res.CreatedAt });
                timeline.Add(new() { Event = "Payment Recorded", OccurredAt = log.CreatedAt });

                if (res.Status is ReservationStatus.CheckedIn or ReservationStatus.CheckedOut)
                    timeline.Add(new() { Event = "Checked In", OccurredAt = res.CheckInDate.ToDateTime(TimeOnly.MinValue) });

                if (res.Status == ReservationStatus.CheckedOut)
                    timeline.Add(new() { Event = "Checked Out", OccurredAt = res.CheckOutDate.ToDateTime(TimeOnly.MinValue) });

                if (res.Status == ReservationStatus.Cancelled && res.CancelledAt.HasValue)
                    timeline.Add(new() { Event = "Cancelled", OccurredAt = res.CancelledAt.Value });
            }
            else
            {
                timeline.Add(new() { Event = "Payment Recorded", OccurredAt = log.CreatedAt });
            }

            return [.. timeline.OrderBy(t => t.OccurredAt)];
        }
    }
}
