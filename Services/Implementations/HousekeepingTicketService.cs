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
    public class HousekeepingTicketService(
        IHousekeepingTicketRepository _ticketRepository,
        IRoomRepository _roomRepository,
        IFacilityRepository _facilityRepository,
        UserManager<ApplicationUser> _userManager,
        ISystemLogService _logService,
        ILogger<HousekeepingTicketService> _logger) : IHousekeepingTicketService

    {
        public async Task<TicketDetailResponse> CreateTicketAsync(
            int hotelId, int createdByUserId, CreateTicketRequest request)
        {
            await ValidateLocationAsync(hotelId, request.LocationType, request.RoomId, request.FacilityId);

            await ValidateAssigneeAsync(hotelId, request.AssignedToId);

            var ticket = new HousekeepingTicket
            {
                HotelId = hotelId,
                Title = request.Title,
                Description = request.Description,
                Priority = request.Priority,
                Status = TicketStatus.Todo,
                Type = request.Type,
                LocationType = request.LocationType,
                RoomId = request.LocationType == TicketLocationType.Room ? request.RoomId : null,
                FacilityId = request.LocationType == TicketLocationType.Facility ? request.FacilityId : null,
                AssignedToId = request.AssignedToId,
                Deadline = request.Deadline,
                CreatedById = createdByUserId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var saved = await _ticketRepository.CreateAsync(ticket);
            _logger.LogInformation("Ticket {TicketId} created for hotel {HotelId} by user {UserId}", saved.Id, hotelId, createdByUserId);
            return new TicketDetailResponse(saved);
        }

        public async Task<TicketDetailResponse> GetTicketByIdAsync(int hotelId, int ticketId)
        {
            var ticket = await _ticketRepository.GetByIdAndHotelIdAsync(ticketId, hotelId)
                ?? throw new HousekeepingTicketNotFoundException(ticketId);

            return new TicketDetailResponse(ticket);
        }

        public async Task<PaginatedResponse<TicketListItemResponse>> GetTicketsByHotelAsync(
            int hotelId, int userId, IList<string> userRoles, TicketListRequest request)
        {
            var (visibleToUserId, effectiveAssignedToId) =
                ResolveVisibility(userId, userRoles, request.AssignedToId);

            var totalCount = await _ticketRepository.CountByHotelIdAsync(
                hotelId, request.Status, request.Type, request.Priority,
                effectiveAssignedToId, visibleToUserId);

            var tickets = await _ticketRepository.GetByHotelIdAsync(
                hotelId, request.Status, request.Type, request.Priority,
                effectiveAssignedToId, request.PageNumber, request.PageSize, visibleToUserId);

            return new PaginatedResponse<TicketListItemResponse>
            {
                Items = [.. tickets.Select(t => new TicketListItemResponse(t))],
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
            };
        }

        public async Task<TicketBoardResponse> GetBoardAsync(int hotelId, int userId, IList<string> userRoles)
        {
            var (visibleToUserId, effectiveAssignedToId) =
                ResolveVisibility(userId, userRoles, null);

            var all = await _ticketRepository.GetAllByHotelIdAsync(hotelId, visibleToUserId);

            // For HousekeepingEmployee (assigned-only), apply the strict filter in-memory
            IEnumerable<HousekeepingTicket> visible = effectiveAssignedToId.HasValue
                ? all.Where(t => t.AssignedToId == effectiveAssignedToId.Value)
                : all;

            return new TicketBoardResponse
            {
                Todo = visible.Where(t => t.Status == TicketStatus.Todo)
                              .Select(t => new TicketListItemResponse(t)).ToList(),
                InProgress = visible.Where(t => t.Status == TicketStatus.InProgress)
                                    .Select(t => new TicketListItemResponse(t)).ToList(),
                Review = visible.Where(t => t.Status == TicketStatus.Review)
                                .Select(t => new TicketListItemResponse(t)).ToList(),
                Done = visible.Where(t => t.Status == TicketStatus.Done)
                              .Select(t => new TicketListItemResponse(t)).ToList()
            };
        }

        public async Task<TicketDetailResponse> UpdateTicketAsync(
            int hotelId, int ticketId, UpdateTicketRequest request)
        {
            var ticket = await _ticketRepository.GetByIdAndHotelIdAsync(ticketId, hotelId)
                ?? throw new HousekeepingTicketNotFoundException(ticketId);

            if (request.LocationType.HasValue || request.RoomId.HasValue || request.FacilityId.HasValue)
            {
                var locType = request.LocationType ?? ticket.LocationType;
                var roomId = request.RoomId ?? ticket.RoomId;
                var facilityId = request.FacilityId ?? ticket.FacilityId;
                await ValidateLocationAsync(hotelId, locType, roomId, facilityId);
                ticket.LocationType = locType;
                ticket.RoomId = locType == TicketLocationType.Room ? roomId : null;
                ticket.FacilityId = locType == TicketLocationType.Facility ? facilityId : null;
            }

            if (request.AssignedToId.HasValue)
                await ValidateAssigneeAsync(hotelId, request.AssignedToId.Value);

            if (request.Title is not null) ticket.Title = request.Title;
            if (request.Description is not null) ticket.Description = request.Description;
            if (request.Priority.HasValue) ticket.Priority = request.Priority.Value;
            if (request.Type.HasValue) ticket.Type = request.Type.Value;
            if (request.AssignedToId is not null) ticket.AssignedToId = request.AssignedToId.Value;
            if (request.Deadline is not null) ticket.Deadline = request.Deadline.Value;
            ticket.UpdatedAt = DateTime.UtcNow;

            var updated = await _ticketRepository.UpdateAsync(ticket);
            _logger.LogInformation("Ticket {TicketId} updated for hotel {HotelId}", ticketId, hotelId);
            return new TicketDetailResponse(updated);
        }

        public async Task<TicketDetailResponse> UpdateTicketStatusAsync(
            int hotelId, int ticketId, UpdateTicketStatusRequest request)
        {
            var ticket = await _ticketRepository.GetByIdAndHotelIdAsync(ticketId, hotelId)
                ?? throw new HousekeepingTicketNotFoundException(ticketId);

            ticket.Status = request.Status;
            ticket.UpdatedAt = DateTime.UtcNow;

            var updated = await _ticketRepository.UpdateAsync(ticket);
            _logger.LogInformation("Ticket {TicketId} status changed to {Status} for hotel {HotelId}", ticketId, request.Status, hotelId);
            return new TicketDetailResponse(updated);
        }

        public async Task DeleteTicketAsync(int hotelId, int ticketId)
        {
            var ticket = await _ticketRepository.GetByIdAndHotelIdAsync(ticketId, hotelId)
                ?? throw new HousekeepingTicketNotFoundException(ticketId);

            await _ticketRepository.DeleteAsync(ticket);

            await _logService.LogAsync(
                SystemLogActions.TicketDeleted,
                SystemLogEntityTypes.Ticket,
                ticketId,
                string.Format(SystemLogMessages.TicketDeleted, ticket.Title),
                hotelId: ticket.HotelId);
            _logger.LogInformation("Ticket {TicketId} deleted for hotel {HotelId}", ticketId, hotelId);
        }

        private async Task ValidateLocationAsync(
            int hotelId, TicketLocationType locationType, int? roomId, int? facilityId)
        {
            if (locationType == TicketLocationType.Room)
            {
                if (!roomId.HasValue)
                    throw new HousekeepingTicketValidationException(
                        "RoomId is required when LocationType is Room.");

                var room = await _roomRepository.GetByIdAsync(roomId.Value)
                    ?? throw new HousekeepingTicketValidationException(
                        $"Room with id '{roomId.Value}' was not found.");

                if (room.HotelId != hotelId)
                    throw new HousekeepingTicketValidationException(
                        $"Room with id '{roomId.Value}' does not belong to hotel with id '{hotelId}'.");
            }
            else
            {
                if (!facilityId.HasValue)
                    throw new HousekeepingTicketValidationException(
                        "FacilityId is required when LocationType is Facility.");

                var facility = await _facilityRepository.GetByIdAsync(facilityId.Value)
                    ?? throw new HousekeepingTicketValidationException(
                        $"Facility with id '{facilityId.Value}' was not found.");

                if (facility.HotelId != hotelId)
                    throw new HousekeepingTicketValidationException(
                        $"Facility with id '{facilityId.Value}' does not belong to hotel with id '{hotelId}'.");
            }
        }

        public async Task<TicketCompletionRateResponse> GetHotelTicketCompletionRateAsync(int hotelId)
        {
            var (total, done) = await _ticketRepository.GetCompletionCountsByHotelIdAsync(hotelId);
            var value = total == 0 ? 0m : Math.Round(done / (decimal)total * DashboardConstants.PercentageMultiplier, DashboardConstants.DecimalPlaces);
            return new TicketCompletionRateResponse { Value = value };
        }

        public async Task<TicketStatusDistributionResponse> GetTicketStatusDistributionAsync(int hotelId)
        {
            var raw = (await _ticketRepository.GetStatusDistributionByHotelIdAsync(hotelId))
                        .ToDictionary(x => x.Status, x => x.Count);

            var allStatuses = new[]
            {
                TicketStatus.Todo,
                TicketStatus.InProgress,
                TicketStatus.Review,
                TicketStatus.Done,
            };

            var total = raw.Values.Sum();

            var items = allStatuses.Select(s =>
            {
                raw.TryGetValue(s, out var count);
                return new TicketStatusSlice
                {
                    Status = s.ToString(),
                    Count = count,
                    Percentage = total == 0 ? 0 : Math.Round(
                        count / (decimal)total * DashboardConstants.PercentageMultiplier,
                        DashboardConstants.DecimalPlaces)
                };
            }).ToList();

            return new TicketStatusDistributionResponse { Total = total, Items = items };
        }

        public async Task<TicketTypeDistributionResponse> GetTicketTypeDistributionAsync(int hotelId)
        {
            var raw = (await _ticketRepository.GetTypeDistributionByHotelIdAsync(hotelId))
                        .ToDictionary(x => x.Type, x => x.Count);

            var allTypes = new[]
            {
                TicketType.Task,
                TicketType.Issue,
                TicketType.Inspection,
                TicketType.MaintenanceRequest,
            };

            var total = raw.Values.Sum();

            var data = allTypes.Select(t =>
            {
                raw.TryGetValue(t, out var count);
                return new TicketTypeSlice { Type = t.ToString(), Count = count };
            }).ToList();

            return new TicketTypeDistributionResponse { Total = total, Data = data };
        }

        public async Task<TicketPriorityDistributionResponse> GetTicketPriorityDistributionAsync(int hotelId)
        {
            var raw = (await _ticketRepository.GetPriorityDistributionByHotelIdAsync(hotelId))
                        .ToDictionary(x => x.Priority, x => x.Count);

            var allPriorities = new[]
            {
                TicketPriority.Low,
                TicketPriority.Medium,
                TicketPriority.High,
                TicketPriority.Urgent,
            };

            var total = raw.Values.Sum();

            var data = allPriorities.Select(p =>
            {
                raw.TryGetValue(p, out var count);
                return new TicketPrioritySlice { Priority = p.ToString(), Count = count };
            }).ToList();

            return new TicketPriorityDistributionResponse { Total = total, Data = data };
        }

        public async Task<HousekeepingKpiResponse> GetHousekeepingKpiAsync(int hotelId)
        {
            var (total, done, overdue, highPriority) = await _ticketRepository.GetKpiCountsByHotelIdAsync(hotelId);
            var active = total - done;
            var rate = total == 0 ? 0m : Math.Round(
                done / (decimal)total * DashboardConstants.PercentageMultiplier,
                DashboardConstants.DecimalPlaces);

            return new HousekeepingKpiResponse
            {
                ActiveTickets       = new KpiCount { Count = active },
                OverdueTickets      = new KpiCount { Count = overdue },
                HighPriorityTickets = new KpiCount { Count = highPriority },
                CompletionRate      = new CompletionRateKpi { Done = done, Total = total, Rate = rate }
            };
        }

        public async Task<OpenTicketsOverTimeResponse> GetOpenTicketsOverTimeAsync(int hotelId, string granularity)
        {
            var today = DateTime.UtcNow.Date;
            var from  = today.AddDays(DashboardConstants.DailyTrendDaysBack);
            var to    = today.AddDays(1).AddTicks(-1);

            var dates = (await _ticketRepository.GetCreationDatesInRangeAsync(hotelId, from, to)).ToList();

            var series = granularity == DashboardConstants.GroupByMonth
                ? BuildMonthlySeries(dates, from, today)
                : granularity == DashboardConstants.GroupByWeek
                    ? BuildWeeklySeries(dates, from, today)
                    : BuildDailySeries(dates, from, today);

            return new OpenTicketsOverTimeResponse
            {
                Granularity = granularity,
                From        = from.ToString(DashboardConstants.IsoDateFormat),
                To          = today.ToString(DashboardConstants.IsoDateFormat),
                Series      = series
            };
        }

        private static List<OpenTicketsTrendItem> BuildDailySeries(
            List<DateTime> dates, DateTime from, DateTime today)
        {
            var grouped = dates.GroupBy(d => d.Date).ToDictionary(g => g.Key, g => g.Count());

            return Enumerable.Range(0, DashboardConstants.DailyTrendDaysCount)
                .Select(i =>
                {
                    var day = from.AddDays(i);
                    grouped.TryGetValue(day, out var count);
                    return new OpenTicketsTrendItem
                    {
                        Date = day.ToString(DashboardConstants.IsoDateFormat),
                        Open = count
                    };
                })
                .ToList();
        }

        private static List<OpenTicketsTrendItem> BuildWeeklySeries(
            List<DateTime> dates, DateTime from, DateTime today)
        {
            var series = new List<OpenTicketsTrendItem>();
            var cursor = from;

            while (cursor <= today)
            {
                var fullEnd   = cursor.AddDays(6);
                var bucketEnd = fullEnd <= today ? fullEnd : today;

                series.Add(new OpenTicketsTrendItem
                {
                    Date = cursor.ToString(DashboardConstants.IsoDateFormat),
                    Open = dates.Count(d => d.Date >= cursor && d.Date <= bucketEnd)
                });

                cursor = cursor.AddDays(7);
            }

            return series;
        }

        private static List<OpenTicketsTrendItem> BuildMonthlySeries(
            List<DateTime> dates, DateTime from, DateTime today)
        {
            var series = new List<OpenTicketsTrendItem>();
            var cursor = from;

            while (cursor <= today)
            {
                var lastOfMonth = new DateTime(cursor.Year, cursor.Month, DateTime.DaysInMonth(cursor.Year, cursor.Month));
                var bucketEnd   = lastOfMonth <= today ? lastOfMonth : today;

                series.Add(new OpenTicketsTrendItem
                {
                    Date = cursor.ToString(DashboardConstants.IsoDateFormat),
                    Open = dates.Count(d => d.Date >= cursor && d.Date <= bucketEnd)
                });

                cursor = new DateTime(cursor.Year, cursor.Month, 1).AddMonths(1);
            }

            return series;
        }

        private async Task ValidateAssigneeAsync(int hotelId, int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user is null || user.HotelId != hotelId)
                throw new HousekeepingTicketValidationException(
                    $"User with id '{userId}' is not a staff member of hotel with id '{hotelId}'.");
        }

        private static (int? visibleToUserId, int? effectiveAssignedToId) ResolveVisibility(
            int userId, IList<string> userRoles, int? requestedAssignedToId)
        {
            if (userRoles.Contains(Roles.AgencyOwner) || userRoles.Contains(Roles.PropertyManager))
                return (null, requestedAssignedToId);

            if (userRoles.Contains(Roles.HousekeepingManager) || userRoles.Contains(Roles.FrontDeskStaff))
                return (userId, requestedAssignedToId);

            if (userRoles.Contains(Roles.HousekeepingEmployee))
                return (null, userId);

            // SuperAdmin and any other elevated roles: no restriction
            return (null, requestedAssignedToId);
        }
    }
}
