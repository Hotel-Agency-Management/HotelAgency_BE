using Booking.Constants;
using Booking.DTO;
using Booking.Exceptions;
using Booking.Interfaces.Repositories;
using Booking.Interfaces.Services;
using Booking.Models;

namespace Booking.Services
{
    public class FacilityService(
        IFacilityRepository _facilityRepository,
        ISystemLogService _logService) : IFacilityService
    {
        public async Task<FacilityResponse> CreateFacilityAsync(int hotelId, CreateFacilityRequest request)
        {

            //var hotel = await _hotelRepository.GetByIdAsync(hotelId)
            //?? throw new HotelNotFoundException(hotelId);

            if (await _facilityRepository.ExistsByNameAndHotelIdAsync(request.Name, hotelId))
                throw new FacilityAlreadyExistsException();

            var facility = new Facility
            {
                HotelId = hotelId,
                Name = request.Name,
                FacilityType = request.FacilityType,
                Description = request.Description,
                Status = request.Status,
                OpenAt = request.OpenAt,
                CloseAt = request.CloseAt,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var saved = await _facilityRepository.CreateAsync(facility);
            return new FacilityResponse(saved);
        }

        public async Task<FacilityResponse> GetFacilityByIdAsync(int facilityId)
        {

            var facility = await _facilityRepository.GetByIdAsync(facilityId)
                ?? throw new FacilityNotFoundException(facilityId);

            return new FacilityResponse(facility);
        }

        public async Task<IEnumerable<FacilityResponse>> GetFacilitiesByHotelIdAsync(int hotelId)
        {
            //var hotel = await _hotelRepository.GetByIdAsync(hotelId)
            //?? throw new HotelNotFoundException(hotelId);

            var facilities = await _facilityRepository.GetAllByHotelIdAsync(hotelId);
            return facilities.Select(f => new FacilityResponse(f));
        }

        public async Task<FacilityResponse> UpdateFacilityAsync(int facilityId, UpdateFacilityRequest request)
        {

            var facility = await _facilityRepository.GetByIdAsync(facilityId)
                ?? throw new FacilityNotFoundException(facilityId);

            if (request.Name is not null) facility.Name = request.Name;
            if (request.FacilityType is not null) facility.FacilityType = request.FacilityType;
            if (request.Description is not null) facility.Description = request.Description;
            if (request.Status is not null) facility.Status = request.Status.Value;
            if (request.OpenAt is not null) facility.OpenAt = request.OpenAt;
            if (request.CloseAt is not null) facility.CloseAt = request.CloseAt;

            facility.UpdatedAt = DateTime.UtcNow;

            var updated = await _facilityRepository.UpdateAsync(facility);
            return new FacilityResponse(updated);
        }

        public async Task DeleteFacilityAsync(int facilityId)
        {
            var facility = await _facilityRepository.GetByIdAsync(facilityId)
                ?? throw new FacilityNotFoundException(facilityId);

            await _facilityRepository.DeleteAsync(facility);

            await _logService.LogAsync(
                SystemLogActions.FacilityDeleted,
                SystemLogEntityTypes.Facility,
                facilityId,
                string.Format(SystemLogMessages.FacilityDeleted, facility.Name),
                hotelId: facility.HotelId);
        }
    }
}
