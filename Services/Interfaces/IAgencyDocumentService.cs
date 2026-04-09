using Booking.DTOs;

namespace Booking.Interfaces.Services
{
    public interface IAgencyDocumentService
    {
        Task<AgencyDocumentResponseDto> UploadDocumentAsync(int agencyId, UploadDocumentDto dto);
        Task<AgencyDocumentResponseDto> UpdateDocumentAsync(int documentId, UpdateDocumentDto dto);
        Task<IEnumerable<AgencyDocumentResponseDto>> GetDocumentsByAgencyAsync(int agencyId);
    }
}
