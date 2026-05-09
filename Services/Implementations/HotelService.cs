using Booking.Constants;
using Booking.DTO;
using Booking.Exceptions;
using Booking.Interfaces.Repositories;
using Booking.Interfaces.Services;
using Booking.Models;
using Booking.Clients;

namespace Booking.Services
{
    public class HotelService(
        IHotelRepository _hotelRepository,
        IAuthRepository _authRepository,
        IBlobStorageService _blobStorageService) : IHotelService
    {
        public async Task<HotelResponse> CreateHotelAsync(CreateHotelRequest request)
        {
            var manager = await GetAvailablePropertyManagerAsync(
                request.AgencyId,
                request.ManagerUserId);

            var logoUrl = await _blobStorageService.UploadAsync(request.Logo);
            var coverPath = await _blobStorageService.UploadAsync(request.CoverPhoto);
            var hotel = new Hotel
            {
                AgencyId = request.AgencyId,
                Name = request.Name,
                Phone = request.Phone,
                Country = request.Country,
                City = request.City,
                Address = request.Address,
                Currency = request.Currency,
                LogoUrl = logoUrl,
                CoverPath = coverPath,
                PrimaryColor = request.PrimaryColor,
                SecondaryColor = request.SecondaryColor,
                TertiaryColor = request.TertiaryColor,
                ManagerUserId = request.ManagerUserId,
                CancellationFeePercentage = request.CancellationFeePercentage,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var saved = await _hotelRepository.CreateAsync(hotel);
            manager.HotelId = saved.Id;
            manager.UpdatedAt = DateTime.UtcNow;

            if (!await _authRepository.UpdateUserAsync(manager))
                throw new TeamMemberUpdateFailedException();

            return new HotelResponse(saved);
        }

        public async Task<HotelResponse> GetHotelByIdAsync(int hotelId)
        {
            var hotel = await _hotelRepository.GetByIdAsync(hotelId)
                ?? throw new HotelNotFoundException(hotelId);

            return new HotelResponse(hotel);
        }

        public async Task<PaginatedResponse<HotelResponse>> GetHotelsByAgencyIdAsync(int agencyId, HotelListRequest request)
        {
            var totalCount = await _hotelRepository.CountByAgencyIdAsync(agencyId, request.Search, request.Location);
            var hotels = await _hotelRepository.GetAllByAgencyIdAsync(
                agencyId, request.Search, request.Location, request.PageNumber, request.PageSize);

            return new PaginatedResponse<HotelResponse>
            {
                Items = [..hotels.Select(h => new HotelResponse(h))],
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
            };
        }

        public async Task<PaginatedResponse<HotelResponse>> GetAllHotelsAsync(HotelListRequest request)
        {
            var totalCount = await _hotelRepository.CountAllAsync(request.Search, request.Location);
            var hotels = await _hotelRepository.GetAllAsync(
                request.Search, request.Location, request.PageNumber, request.PageSize);

            return new PaginatedResponse<HotelResponse>
            {
                Items = [..hotels.Select(h => new HotelResponse(h))],
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
            };
        }

        public async Task<HotelResponse> UpdateHotelAsync(int hotelId, UpdateHotelRequest request)
        {

            var hotel = await _hotelRepository.GetByIdAsync(hotelId)
                ?? throw new HotelNotFoundException(hotelId);

            if (request.Name is not null) hotel.Name = request.Name;
            if (request.Phone is not null) hotel.Phone = request.Phone;
            if (request.Country is not null) hotel.Country = request.Country;
            if (request.City is not null) hotel.City = request.City;
            if (request.Address is not null) hotel.Address = request.Address;
            if (request.Currency is not null) hotel.Currency = request.Currency;
            if (request.PrimaryColor is not null) hotel.PrimaryColor = request.PrimaryColor;
            if (request.SecondaryColor is not null) hotel.SecondaryColor = request.SecondaryColor;
            if (request.TertiaryColor is not null) hotel.TertiaryColor = request.TertiaryColor;
            if (request.CancellationFeePercentage.HasValue)
                hotel.CancellationFeePercentage = request.CancellationFeePercentage.Value;
            if (request.ManagerUserId is not null && request.ManagerUserId.Value != hotel.ManagerUserId)
                await AssignNewManagerAsync(hotel, request.ManagerUserId.Value);

            if (request.Logo is not null)
            {
                await _blobStorageService.DeleteAsync(hotel.LogoUrl);
                hotel.LogoUrl = await _blobStorageService.UploadAsync(request.Logo);
            }

            if (request.CoverPhoto is not null)
            {
                await _blobStorageService.DeleteAsync(hotel.CoverPath);
                hotel.CoverPath = await _blobStorageService.UploadAsync(request.CoverPhoto);
            }

            hotel.UpdatedAt = DateTime.UtcNow;

            var updated = await _hotelRepository.UpdateAsync(hotel);
            return new HotelResponse(updated);
        }

        private async Task<ApplicationUser> GetAvailablePropertyManagerAsync(int agencyId, int managerUserId)
        {
            var manager = await _authRepository.FindByIdAsync(managerUserId);
            if (manager is null)
                throw new ManagerUserNotFoundException(managerUserId);

            if (manager.AgencyId != agencyId)
                throw new ManagerDoesNotBelongToAgencyException(managerUserId, agencyId);

            var role = await _authRepository.GetRoleAsync(manager);
            if (role != Roles.PropertyManager)
                throw new ManagerMustBePropertyManagerException(managerUserId);

            if (manager.HotelId.HasValue)
                throw new ManagerAlreadyAssignedToHotelException(managerUserId, manager.HotelId.Value);

            return manager;
        }

        private async Task AssignNewManagerAsync(Hotel hotel, int managerUserId)
        {
            var newManager = await GetAvailablePropertyManagerAsync(hotel.AgencyId, managerUserId);

            var oldManager = await _authRepository.FindByIdAsync(hotel.ManagerUserId);
            if (oldManager is not null && oldManager.HotelId == hotel.Id)
            {
                oldManager.HotelId = null;
                oldManager.UpdatedAt = DateTime.UtcNow;

                if (!await _authRepository.UpdateUserAsync(oldManager))
                    throw new TeamMemberUpdateFailedException();
            }

            newManager.HotelId = hotel.Id;
            newManager.UpdatedAt = DateTime.UtcNow;

            if (!await _authRepository.UpdateUserAsync(newManager))
                throw new TeamMemberUpdateFailedException();

            hotel.ManagerUserId = managerUserId;
        }
    }
}
