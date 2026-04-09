using Booking.Clients;
using Booking.DTOs;
using Booking.Exceptions;
using Booking.Models;
using Booking.Interfaces.Repositories;
using Booking.Interfaces.Services;
using Booking.Enums;

namespace Booking.Services
{
    public class AgencyDocumentService(
        IAgencyDocumentRepository _documentRepository,
        IBlobStorageService _blobStorageService,
        IAgencyRepository _agencyRepository) : IAgencyDocumentService
    {
        public async Task<AgencyDocumentResponseDto> UploadDocumentAsync(int agencyId, UploadDocumentDto dto)
        {
            var fileName = await _blobStorageService.UploadAsync(dto.File);

            var document = new AgencyDocument
            {
                AgencyId = agencyId,
                DocumentType = dto.DocumentType,
                FilePath = fileName,
                UploadedAt = DateTime.UtcNow
            };

            var saved = await _documentRepository.AddAsync(document);

            var documentCount = await _documentRepository.CountByAgencyIdAsync(agencyId);
            if (documentCount == 1)
            {
                await _agencyRepository.UpdateStatusAsync(agencyId, AgencyStatus.Pending);
            }

            return new AgencyDocumentResponseDto(saved);
        }

        public async Task<AgencyDocumentResponseDto> UpdateDocumentAsync(int documentId, UpdateDocumentDto dto)
        {
            var existing = await _documentRepository.GetByIdAsync(documentId)
                ?? throw new AgencyDocumentNotFoundException(documentId);

            await _blobStorageService.DeleteAsync(existing.FilePath);

            var newFileName = await _blobStorageService.UploadAsync(dto.File);

            existing.FilePath = newFileName;
            existing.UploadedAt = DateTime.UtcNow;

            if (!string.IsNullOrWhiteSpace(dto.DocumentType))
                existing.DocumentType = dto.DocumentType;

            var updated = await _documentRepository.UpdateAsync(existing);
            return new AgencyDocumentResponseDto(updated);
        }

        public async Task<IEnumerable<AgencyDocumentResponseDto>> GetDocumentsByAgencyAsync(int agencyId)
        {
            var documents = await _documentRepository.GetByAgencyIdAsync(agencyId);
            return documents.Select(d => new AgencyDocumentResponseDto(d));
        }
    }
}
