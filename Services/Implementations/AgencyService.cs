using Booking.Clients;
using Booking.DTO;
using Booking.Exceptions;
using Booking.Interfaces.Repositories;
using Booking.Interfaces.Services;
using Booking.Models;
using System.Globalization;

namespace Booking.Services
{
    public class AgencyService(
        IAgencyRepository _agencyRepository,
        IBlobStorageService _blobStorageService,
        ILogger<AgencyService> _logger) : IAgencyService
    {
        public async Task<PaginatedResponse<AgencyListItemResponse>> GetAllAgenciesAsync(AgencyListRequest request)
        {
            _logger.LogDebug("Fetching agencies page {Page} limit {Limit}", request.Page, request.Limit);
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

        public async Task<AgencyStatusCountsResponse> GetAgencyStatusCountsAsync()
        {
            return await _agencyRepository.GetStatusCountsAsync();
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
            _logger.LogInformation("Agency {AgencyId} updated successfully", agencyId);
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

            _logger.LogInformation("Agency {AgencyId} logo updated", agencyId);
            return logoUrl;
        }

        public async Task<AgencyGrowthResponse> GetMonthlyGrowthAsync()
        {
            var now = DateTime.UtcNow;
            var windowStart = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc).AddMonths(-11);
            var windowEnd = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc).AddMonths(1).AddTicks(-1);

            var raw = (await _agencyRepository.GetMonthlyGrowthAsync(windowStart, windowEnd))
                .ToDictionary(r => (r.Year, r.Month), r => r.Count);

            var months = Enumerable.Range(0, 12)
                .Select(i => windowStart.AddMonths(i))
                .Select(d => new AgencyGrowthItem
                {
                    Month = d.ToString("MMM yyyy", CultureInfo.InvariantCulture),
                    Count = raw.TryGetValue((d.Year, d.Month), out var c) ? c : 0
                })
                .ToList();

            return new AgencyGrowthResponse { Growth = months };
        }
    }
}
