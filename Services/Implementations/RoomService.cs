using Booking.Clients;
using Booking.Constants;
using Booking.DTO;
using Booking.Exceptions;
using Booking.Interfaces.Repositories;
using Booking.Interfaces.Services;
using Booking.Models;

namespace Booking.Services
{
    public class RoomService(
        IRoomRepository _roomRepository,
        IHotelRepository _hotelRepository,
        IBlobStorageService _blobStorageService,
        IRoomTypeRepository _roomTypeRepository) : IRoomService
    {
        public async Task<RoomResponse> CreateRoomAsync(int hotelId, CreateRoomRequest request)
        {
            var hotel = await _hotelRepository.GetByIdAsync(hotelId)
                ?? throw new HotelNotFoundException(hotelId);

            var roomType = await _roomTypeRepository.GetByIdAsync(request.RoomTypeId)
                ?? throw new RoomTypeNotInHotelException();
            var CoverPhotoUrl = await _blobStorageService.UploadAsync(request.coverPhoto);

            if (await _roomRepository.ExistsByRoomNumberAndHotelIdAsync(request.RoomNumber, hotelId))
                throw new RoomAlreadyExistsException();

            var room = new Room
            {
                HotelId = hotelId,
                RoomTypeId = request.RoomTypeId,
                RoomNumber = request.RoomNumber,
                FloorNumber = request.FloorNumber,
                Description = request.Description,
                Status = request.Status,
                Notes = request.Notes,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CoverPhotoUrl = CoverPhotoUrl
            };

            var saved = await _roomRepository.CreateAsync(room);
            return new RoomResponse(saved);
        }

        public async Task<RoomResponse> GetRoomByIdAsync(int hotelId, int roomId)
        {
            var room = await _roomRepository.GetByIdAndHotelIdAsync(roomId, hotelId)
                ?? throw new RoomNotFoundException(roomId);

            return new RoomResponse(room);
        }

        public async Task<IEnumerable<RoomResponse>> GetRoomsByHotelIdAsync(int hotelId)
        {
            var hotel = await _hotelRepository.GetByIdAsync(hotelId)
                ?? throw new HotelNotFoundException(hotelId);

            var rooms = await _roomRepository.GetAllByHotelIdAsync(hotelId);
            return rooms.Select(r => new RoomResponse(r));
        }

        public async Task<RoomResponse> UpdateRoomAsync(int hotelId, int roomId, UpdateRoomRequest request)
        {
            var room = await _roomRepository.GetByIdAndHotelIdAsync(roomId, hotelId)
                ?? throw new RoomNotFoundException(roomId);

            if (request.RoomTypeId is not null)
            {
                var roomType = await _roomTypeRepository.GetByIdAsync(request.RoomTypeId.Value)
                    ?? throw new RoomTypeNotInHotelException();

                room.RoomTypeId = request.RoomTypeId.Value;
            }

            if (request.RoomNumber is not null) room.RoomNumber = request.RoomNumber;
            if (request.FloorNumber is not null) room.FloorNumber = request.FloorNumber.Value;
            if (request.Description is not null) room.Description = request.Description;
            if (request.Status is not null) room.Status = request.Status.Value;
            if (request.Notes is not null) room.Notes = request.Notes;
            if (request.CoverPhoto is not null)
            {
                if (room.CoverPhotoUrl is not null)
                    await _blobStorageService.DeleteAsync(room.CoverPhotoUrl);
                room.CoverPhotoUrl = await _blobStorageService.UploadAsync(request.CoverPhoto);
            }

            room.UpdatedAt = DateTime.UtcNow;

            var updated = await _roomRepository.UpdateAsync(room);
            return new RoomResponse(updated);
        }

        public async Task DeleteRoomAsync(int hotelId, int roomId)
        {
            var room = await _roomRepository.GetByIdAndHotelIdAsync(roomId, hotelId)
                ?? throw new RoomNotFoundException(roomId);

            await _roomRepository.DeleteAsync(room);
            if (room.CoverPhotoUrl is not null)
                await _blobStorageService.DeleteAsync(room.CoverPhotoUrl);
        }
    }
}
