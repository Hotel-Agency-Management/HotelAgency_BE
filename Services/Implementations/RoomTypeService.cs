using Booking.Constants;
using Booking.DTO;
using Booking.Exceptions;
using Booking.Interfaces.Repositories;
using Booking.Interfaces.Services;
using Booking.Models;

namespace Booking.Services
{
    public class RoomTypeService(
        IRoomTypeRepository _roomTypeRepository,
        ISystemLogService _logService,
        ILogger<RoomTypeService> _logger) : IRoomTypeService
    {
        public async Task<RoomTypeResponse> CreateRoomTypeAsync(CreateRoomTypeRequest request)
        {
            if (await _roomTypeRepository.ExistsByNameAsync(request.Name))
                throw new RoomTypeAlreadyExistsException();

            var roomType = new RoomType
            {
                Name = request.Name,
                Description = request.Description,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var saved = await _roomTypeRepository.CreateAsync(roomType);
            _logger.LogInformation("Room type {RoomTypeId} created: {Name}", saved.Id, request.Name);
            return new RoomTypeResponse(saved);
        }

        public async Task<RoomTypeResponse> GetRoomTypeByIdAsync(int roomTypeId)
        {
            var roomType = await _roomTypeRepository.GetByIdAsync(roomTypeId)
                ?? throw new RoomTypeNotFoundException(roomTypeId);

            return new RoomTypeResponse(roomType);
        }

        public async Task<IEnumerable<RoomTypeResponse>> GetRoomTypesAsync()
        {
            var roomTypes = await _roomTypeRepository.GetAllAsync();
            return roomTypes.Select(r => new RoomTypeResponse(r));
        }

        public async Task<RoomTypeResponse> UpdateRoomTypeAsync(int roomTypeId, UpdateRoomTypeRequest request)
        {
            var roomType = await _roomTypeRepository.GetByIdAsync(roomTypeId)
                ?? throw new RoomTypeNotFoundException(roomTypeId);

            if (request.Name is not null) roomType.Name = request.Name;
            if (request.Description is not null) roomType.Description = request.Description;

            roomType.UpdatedAt = DateTime.UtcNow;

            var updated = await _roomTypeRepository.UpdateAsync(roomType);
            _logger.LogInformation("Room type {RoomTypeId} updated", roomTypeId);
            return new RoomTypeResponse(updated);
        }

        public async Task DeleteRoomTypeAsync(int roomTypeId)
        {
            var roomType = await _roomTypeRepository.GetByIdAsync(roomTypeId)
                ?? throw new RoomTypeNotFoundException(roomTypeId);

            await _roomTypeRepository.DeleteAsync(roomType);

            await _logService.LogAsync(
                SystemLogActions.RoomTypeDeleted,
                SystemLogEntityTypes.RoomType,
                roomTypeId,
                string.Format(SystemLogMessages.RoomTypeDeleted, roomType.Name));
            _logger.LogInformation("Room type {RoomTypeId} deleted", roomTypeId);

        }
    }
}
