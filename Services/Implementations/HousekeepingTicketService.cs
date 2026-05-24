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
        UserManager<ApplicationUser> _userManager) : IHousekeepingTicketService
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
            return new TicketDetailResponse(updated);
        }

        public async Task DeleteTicketAsync(int hotelId, int ticketId)
        {
            var ticket = await _ticketRepository.GetByIdAndHotelIdAsync(ticketId, hotelId)
                ?? throw new HousekeepingTicketNotFoundException(ticketId);

            await _ticketRepository.DeleteAsync(ticket);
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
