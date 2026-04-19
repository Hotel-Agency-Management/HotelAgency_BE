using Booking.Constants;
using Booking.DTO;
using Booking.Exceptions;
using Booking.Interfaces.Repositories;
using Booking.Interfaces.Services;
using Booking.Models;

namespace Booking.Services
{
    public class RoomTypeService(
        IRoomTypeRepository _roomTypeRepository) : IRoomTypeService
    {
        public async Task<RoomTypeResponse> CreateRoomTypeAsync(CreateRoomTypeRequest request)
        {
            if (await _roomTypeRepository.ExistsByNameAsync(request.Name))
                throw new RoomTypeAlreadyExistsException();

            var roomType = new RoomType
            {
                Name = request.Name,
                Description = request.Description,
                Capacity = request.Capacity,
                DailyPrice = request.DailyPrice,
                WeeklyPrice = request.WeeklyPrice,
                MonthlyPrice = request.MonthlyPrice,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var saved = await _roomTypeRepository.CreateAsync(roomType);
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
            if (request.Capacity is not null) roomType.Capacity = request.Capacity.Value;
            if (request.DailyPrice is not null) roomType.DailyPrice = request.DailyPrice.Value;
            if (request.WeeklyPrice is not null) roomType.WeeklyPrice = request.WeeklyPrice.Value;
            if (request.MonthlyPrice is not null) roomType.MonthlyPrice = request.MonthlyPrice.Value;

            roomType.UpdatedAt = DateTime.UtcNow;

            var updated = await _roomTypeRepository.UpdateAsync(roomType);
            return new RoomTypeResponse(updated);
        }

        public async Task DeleteRoomTypeAsync(int roomTypeId)
        {
            var roomType = await _roomTypeRepository.GetByIdAsync(roomTypeId)
                ?? throw new RoomTypeNotFoundException(roomTypeId);

            await _roomTypeRepository.DeleteAsync(roomType);
        }
    }
}
