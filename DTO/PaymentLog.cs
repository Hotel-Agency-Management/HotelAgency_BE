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
    }

    public class PaymentLogResponse
    {
        public int Id { get; set; }
        public int? ReservationId { get; set; }
        public decimal Amount { get; set; }
        public string Type { get; set; } = string.Empty;
        public int From { get; set; }
        public int To { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class PaymentLogExpenseResponse
    {
        public IReadOnlyCollection<PaymentLogResponse> Items { get; set; } = [];
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
