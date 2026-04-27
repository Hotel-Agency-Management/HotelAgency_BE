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
            var logoUrl = await _blobStorageService.UploadAsync(request.Logo);
            var coverPath = await _blobStorageService.UploadAsync(request.CoverPhoto);

            var manager = await _authRepository.FindByIdAsync(request.ManagerUserId);
            if (manager is null)
                throw new ManagerUserNotFoundException(request.ManagerUserId);


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
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var saved = await _hotelRepository.CreateAsync(hotel);
            return new HotelResponse(saved);
        }

        public async Task<HotelResponse> GetHotelByIdAsync(int hotelId)
        {
            var hotel = await _hotelRepository.GetByIdAsync(hotelId)
                ?? throw new HotelNotFoundException(hotelId);

            return new HotelResponse(hotel);
        }

        public async Task<IEnumerable<HotelResponse>> GetHotelsByAgencyIdAsync(int agencyId)
        {
            var hotels = await _hotelRepository.GetAllByAgencyIdAsync(agencyId);
            return hotels.Select(h => new HotelResponse(h));
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
            if (request.ManagerUserId is not null) hotel.ManagerUserId = request.ManagerUserId.Value;

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
    }
}
