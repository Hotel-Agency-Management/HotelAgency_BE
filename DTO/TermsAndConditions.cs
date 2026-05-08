using System.ComponentModel.DataAnnotations;
using Booking.Enums;
using Booking.Models;

namespace Booking.DTO
{
    public class CreateTermsRequest
    {
        [Required] public string Title { get; set; } = string.Empty;
        [Required] public string Content { get; set; } = string.Empty;
        public TermsStatus Status { get; set; } = TermsStatus.Draft;
    }

    public class UpdateTermsRequest
    {
        public string? Title { get; set; }
        public string? Content { get; set; }
        public TermsStatus? Status { get; set; }
    }

    public class TermsResponse
    {
        public int Id { get; set; }
        public int HotelId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public TermsStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public TermsResponse(TermsAndConditions terms)
        {
            Id = terms.Id;
            HotelId = terms.HotelId;
            Title = terms.Title;
            Content = terms.Content;
            Status = terms.Status;
            CreatedAt = terms.CreatedAt;
            UpdatedAt = terms.UpdatedAt;
        }
    }
}
