using System.ComponentModel.DataAnnotations;
using Booking.Enums;
using Booking.Models;

namespace Booking.DTO
{
    public class TicketCommentListRequest
    {
        [Range(1, int.MaxValue)]
        public int PageNumber { get; set; } = 1;

        [Range(1, 100)]
        public int PageSize { get; set; } = 10;
    }

    public class CreateTicketCommentRequest
    {
        [Required]
        public CommentType CommentType { get; set; }

        [Required]
        [MaxLength(2000)]
        public string Content { get; set; } = string.Empty;

        public decimal? DamageCost { get; set; }
    }

    public class UpdateTicketCommentRequest
    {
        [MaxLength(2000)]
        public string? Content { get; set; }

        public decimal? DamageCost { get; set; }
    }

    public class TicketCommentResponse
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public int AuthorId { get; set; }
        public string? AuthorName { get; set; }
        public string CommentType { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public decimal? DamageCost { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public TicketCommentResponse(TicketComment c)
        {
            Id = c.Id;
            TicketId = c.TicketId;
            AuthorId = c.AuthorId;
            AuthorName = c.Author is null
                ? null
                : $"{c.Author.FirstName} {c.Author.LastName}";
            CommentType = c.CommentType.ToString();
            Content = c.Content;
            DamageCost = c.DamageCost;
            CreatedAt = c.CreatedAt;
            UpdatedAt = c.UpdatedAt;
        }
    }
}
