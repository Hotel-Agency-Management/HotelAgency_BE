using Booking.Clients;
using Booking.DTO;
using Booking.Enums;
using Booking.Exceptions;
using Booking.Interfaces.Repositories;
using Booking.Interfaces.Services;
using Booking.Models;
using Microsoft.AspNetCore.Http;

namespace Booking.Services
{
    public class AgencyService(
        IAgencyRepository _agencyRepository,
        IBlobStorageService _blobStorageService) : IAgencyService
    {
        public async Task<PaginatedResponse<AgencyListItemResponse>> GetAllAgenciesAsync(AgencyListRequest request)
        {
            var (agencies, totalCount) = await _agencyRepository.GetAllAsync(request);

            return new PaginatedResponse<AgencyListItemResponse>
            {
                Items = agencies.Select(agency => new AgencyListItemResponse(agency)).ToList(),
                PageNumber = request.Page,
                PageSize = request.Limit,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)request.Limit)
            };
        }

        public async Task<Agency> GetAgencyProfileAsync(int agencyId)
        {
            var agency = await _agencyRepository.GetByIdAsync(agencyId);
            return agency!;
        }

        public async Task UpdateAgencyAsync(int agencyId, UpdateAgencyRequest request)
        {
            var agency = await _agencyRepository.GetByIdAsync(agencyId);
            if (agency is null)
                throw new AgencyNotFoundException(agencyId);

            if (!string.IsNullOrWhiteSpace(request.AgencyName))
                agency.AgencyName = request.AgencyName.Trim();

            if (!string.IsNullOrWhiteSpace(request.Phone))
                agency.Phone = request.Phone.Trim();

            if (!string.IsNullOrWhiteSpace(request.Country))
                agency.Country = request.Country.Trim();

            if (!string.IsNullOrWhiteSpace(request.City))
                agency.City = request.City.Trim();

            if (request.PrimaryColor is not null)
                agency.PrimaryColor = request.PrimaryColor;

            if (request.SecondaryColor is not null)
                agency.SecondaryColor = request.SecondaryColor;

            if (request.TertiaryColor is not null)
                agency.TertiaryColor = request.TertiaryColor;

            agency.UpdatedAt = DateTime.UtcNow;

            await _agencyRepository.UpdateAsync(agency);
        }

        public async Task<string> UpdateAgencyLogoAsync(int agencyId, IFormFile file)
        {
            var agency = await _agencyRepository.GetByIdAsync(agencyId)
                ?? throw new AgencyNotFoundException(agencyId);

            if (!string.IsNullOrEmpty(agency.LogoUrl))
                await _blobStorageService.DeleteAsync(agency.LogoUrl);

            var logoUrl = await _blobStorageService.UploadAsync(file);

            agency.LogoUrl = logoUrl;
            agency.UpdatedAt = DateTime.UtcNow;
            await _agencyRepository.UpdateAsync(agency);

            return logoUrl;
        }
    }
}
