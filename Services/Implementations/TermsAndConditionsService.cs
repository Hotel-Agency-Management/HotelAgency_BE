using Booking.DTO;
using Booking.Enums;
using Booking.Exceptions;
using Booking.Interfaces.Repositories;
using Booking.Interfaces.Services;
using Booking.Models;

namespace Booking.Services
{
    public class TermsAndConditionsService(
        ITermsAndConditionsRepository _termsRepository,
        ILogger<TermsAndConditionsService> _logger) : ITermsAndConditionsService
    {
        public async Task<TermsResponse> CreateTermsAsync(int hotelId, CreateTermsRequest dto)
        {
            if (dto.Status == TermsStatus.Active)
                await _termsRepository.SetAllToInactiveForHotelAsync(hotelId);

            var terms = new TermsAndConditions
            {
                HotelId = hotelId,
                Title = dto.Title,
                Content = dto.Content,
                Status = dto.Status,
            };

            var created = await _termsRepository.CreateAsync(terms);
            _logger.LogInformation("Terms {TermsId} created for hotel {HotelId}", created.Id, hotelId);
            return new TermsResponse(created);
        }

        public async Task<IEnumerable<TermsResponse>> GetTermsByHotelIdAsync(int hotelId)
        {
            var terms = await _termsRepository.GetAllByHotelIdAsync(hotelId);
            return terms.Select(t => new TermsResponse(t));
        }

        public async Task<TermsResponse> GetTermsByIdAsync(int id)
        {
            var terms = await _termsRepository.GetByIdAsync(id)
                ?? throw new TermsNotFoundException(id);
            return new TermsResponse(terms);
        }

        public async Task<TermsResponse> UpdateTermsAsync(int id, UpdateTermsRequest dto)
        {
            var terms = await _termsRepository.GetByIdAsync(id)
                ?? throw new TermsNotFoundException(id);

            if (dto.Status == TermsStatus.Active && terms.Status != TermsStatus.Active)
                await _termsRepository.SetAllToInactiveForHotelAsync(terms.HotelId, excludeId: id);

            if (dto.Title is not null) terms.Title = dto.Title;
            if (dto.Content is not null) terms.Content = dto.Content;
            if (dto.Status.HasValue) terms.Status = dto.Status.Value;

            var updated = await _termsRepository.UpdateAsync(terms);
            _logger.LogInformation("Terms {TermsId} updated", id);
            return new TermsResponse(updated);
        }
    }
}
