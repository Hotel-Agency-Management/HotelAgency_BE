using System.ComponentModel.DataAnnotations;
using Booking.Enums;

namespace Booking.DTO
{
    public class PaymentLogListRequest
    {
        [Range(1, int.MaxValue)]
        public int PageNumber { get; set; } = 1;

        [Range(1, 100)]
        public int PageSize { get; set; } = 10;

        public PaymentType? Type { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string SortOrder { get; set; } = "Newest";
    }

    public class PaymentLogSummaryResponse
    {
        public decimal TotalIncoming { get; set; }
        public decimal TotalOutgoing { get; set; }
        public int IncomingCount { get; set; }
        public int OutgoingCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public IReadOnlyCollection<PaymentLogGroupResponse> Groups { get; set; } = [];
    }

    public class PaymentLogGroupResponse
    {
        public DateOnly WeekStart { get; set; }
        public DateOnly WeekEnd { get; set; }
        public IReadOnlyCollection<PaymentLogItemResponse> Items { get; set; } = [];
    }

    public class PaymentLogItemResponse
    {
        public int PaymentId { get; set; }
        public string? ReservationReference { get; set; }
        public string PaymentType { get; set; } = string.Empty;
        public string? Reason { get; set; }
        public decimal Amount { get; set; }
        public string FromName { get; set; } = string.Empty;
        public string ToName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class PaymentLogDetailsResponse
    {
        public int PaymentId { get; set; }
        public string PaymentType { get; set; } = string.Empty;
        public string? Reason { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? ReservationReference { get; set; }
        public int? ReservationId { get; set; }
        public string FromName { get; set; } = string.Empty;
        public string ToName { get; set; } = string.Empty;
        public IReadOnlyCollection<PaymentTimelineItem> Timeline { get; set; } = [];
    }

    public class PaymentTimelineItem
    {
        public string Event { get; set; } = string.Empty;
        public DateTime OccurredAt { get; set; }
    }
}
