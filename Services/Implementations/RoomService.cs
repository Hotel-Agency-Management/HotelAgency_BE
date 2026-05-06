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
        IBlobStorageService _blobStorageService,
        IRoomAmenityRepository _roomAmenityRepository,
        IRoomTypeRepository _roomTypeRepository) : IRoomService
    {
        public async Task<RoomResponse> CreateRoomAsync(int hotelId, CreateRoomRequest request)
        {
            var roomType = await _roomTypeRepository.GetByIdAsync(request.RoomTypeId)
                ?? throw new RoomTypeNotInHotelException();
            var CoverPhotoUrl = await _blobStorageService.UploadAsync(request.coverPhoto);

            if (await _roomRepository.ExistsByRoomNumberAndHotelIdAsync(request.RoomNumber, hotelId))
                throw new RoomAlreadyExistsException();


            var amenities = new List<RoomAmenity>();
            if (request.AmenityIds.Any())
            {
                amenities = await _roomAmenityRepository.GetByIdsAsync(request.AmenityIds);

                var missingIds = request.AmenityIds
                    .Except(amenities.Select(a => a.Id))
                    .ToList();

                if (missingIds.Any())
                    throw new SomeAmenitiesNotFoundException(missingIds);
            }

            var room = new Room
            {
                HotelId = hotelId,
                RoomTypeId = request.RoomTypeId,
                RoomNumber = request.RoomNumber,
                FloorNumber = request.FloorNumber,
                Description = request.Description,
                Status = request.Status,
                Notes = request.Notes,
                DailyPrice = request.DailyPrice,
                MonthlyPrice = request.MonthlyPrice,
                WeeklyPrice = request.WeeklyPrice,
                ExtendPrice = request.ExtendPrice,
                Capacity = request.Capacity,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CoverPhotoUrl = CoverPhotoUrl,
                Amenities = amenities
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

        public async Task<PaginatedResponse<HotelRoomResponse>> GetFilteredRoomsByHotelIdAsync(
            int hotelId, GetHotelRoomsRequest request)
        {
            if (request.CheckIn.HasValue && request.CheckOut.HasValue &&
                request.CheckOut <= request.CheckIn)
                throw new BadRequestException("Check-out date must be after check-in date.");

            var (rooms, totalCount) = await _roomRepository.GetFilteredByHotelIdAsync(hotelId, request);

            return new PaginatedResponse<HotelRoomResponse>
            {
                Items = rooms.Select(r => new HotelRoomResponse(r)).ToList(),
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
            };
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
            if (request.Capacity is not null) room.Capacity = request.Capacity.Value;
            if (request.DailyPrice is not null) room.DailyPrice = request.DailyPrice.Value;
            if (request.WeeklyPrice is not null) room.WeeklyPrice = request.WeeklyPrice.Value;
            if (request.MonthlyPrice is not null) room.MonthlyPrice = request.MonthlyPrice.Value;
            if (request.ExtendPrice is not null) room.ExtendPrice = request.ExtendPrice.Value;
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
