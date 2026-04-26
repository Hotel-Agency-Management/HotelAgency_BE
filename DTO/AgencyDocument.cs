using Booking.Models;

namespace Booking.DTO
{
    public class UploadDocumentDto
    {
        public string DocumentType { get; set; } = string.Empty;
        public IFormFile File { get; set; } = null!;
    }

    public class UpdateDocumentDto
    {
        public string? DocumentType { get; set; }
        public IFormFile? File { get; set; } = null!;
    }

    public class AgencyDocumentResponseDto
    {
        public int Id { get; set; }
        public string DocumentType { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public int AgencyId { get; set; }
        public DateTime UploadedAt { get; set; }

        public AgencyDocumentResponseDto(AgencyDocument document)
        {
            Id = document.Id;
            DocumentType = document.DocumentType;
            FilePath = document.FilePath;
            AgencyId = document.AgencyId;
            UploadedAt = document.UploadedAt;
        }
    }
}
