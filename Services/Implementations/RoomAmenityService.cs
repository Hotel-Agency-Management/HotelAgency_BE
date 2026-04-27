using Booking.Constants;
using Booking.DTO;
using Booking.Exceptions;
using Booking.Interfaces.Repositories;
using Booking.Interfaces.Services;
using Booking.Models;

namespace Booking.Services
{
    public class RoomAmenityService(
        IRoomAmenityRepository _amenityRepository) : IRoomAmenityService
    {
        public async Task<RoomAmenityResponse> CreateAmenityAsync(CreateRoomAmenityRequest request)
        {
            if (await _amenityRepository.ExistsByNameAsync(request.Name))
                throw new RoomAmenityAlreadyExistsException();

            var amenity = new RoomAmenity
            {
                Name = request.Name
            };

            var saved = await _amenityRepository.CreateAsync(amenity);
            return new RoomAmenityResponse(saved);
        }

        public async Task<RoomAmenityResponse> GetAmenityByIdAsync(int amenityId)
        {
            var amenity = await _amenityRepository.GetByIdAsync(amenityId)
                ?? throw new RoomAmenityNotFoundException(amenityId);

            return new RoomAmenityResponse(amenity);
        }

        public async Task<IEnumerable<RoomAmenityResponse>> GetAllAmenitiesAsync()
        {
            var amenities = await _amenityRepository.GetAllAsync();
            return amenities.Select(a => new RoomAmenityResponse(a));
        }

        public async Task DeleteAmenityAsync(int amenityId)
        {
            var amenity = await _amenityRepository.GetByIdAsync(amenityId)
                ?? throw new RoomAmenityNotFoundException(amenityId);

            await _amenityRepository.DeleteAsync(amenity);
        }
    }
}
