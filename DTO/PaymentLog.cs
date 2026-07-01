using System.ComponentModel.DataAnnotations;
using Booking.Constants;
using Booking.Enums;

namespace Booking.DTO
{
    public class CreatePaymentLogRequest
    {
        public int? ReservationId { get; set; }

        [Required, Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }

        [Required]
        public PaymentType Type { get; set; }

        public string? Reason { get; set; }

        public int? From { get; set; }

        public int? To { get; set; }
    }

    public class UpdatePaymentLogRequest
    {
        [Range(0.01, double.MaxValue)]
        public decimal? Amount { get; set; }

        public PaymentType? Type { get; set; }

        public string? Reason { get; set; }
    }

    public class PaymentLogListRequest
    {
        [Range(1, int.MaxValue)]
        public int PageNumber { get; set; } = 1;

        [Range(1, 100)]
        public int PageSize { get; set; } = 10;

        public PaymentType? Type { get; set; }
        public PaymentDirection? Direction { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string SortOrder { get; set; } = SortOrders.Newest;
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
        public string TransactionType { get; set; } = string.Empty;
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

    public class CashFlowItem
    {
        public string Month { get; set; } = string.Empty;
        public int Year { get; set; }
        public decimal Incoming { get; set; }
        public decimal Outgoing { get; set; }
    }

    public class RevenueByTypeItem
    {
        public string PaymentType { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
    }

    public class BalanceTrendItem
    {
        public string Month { get; set; } = string.Empty;
        public int Year { get; set; }
        public decimal Balance { get; set; }
    }

    public class RefundImpactResponse
    {
        public decimal PaidRevenue { get; set; }
        public decimal RefundAmount { get; set; }
        public decimal CancellationLoss { get; set; }
    }

    public class RevenueGrowthResponse
    {
        public decimal CurrentRevenue   { get; set; }
        public decimal PreviousRevenue  { get; set; }
        public decimal GrowthPercentage { get; set; }
        public decimal GaugeScore       { get; set; }
        public int Month { get; set; }
        public int Year  { get; set; }
    }

    public class FinancialSummaryResponse
    {
        public decimal TotalRevenue        { get; set; }
        public decimal TotalExpenses       { get; set; }
        public decimal NetProfit           { get; set; }
        public decimal OutstandingPayments { get; set; }
        public decimal Refunds             { get; set; }
        public decimal CashBalance         { get; set; }
    }

    public class RevenueExpensesItem
    {
        public string  Month    { get; set; } = string.Empty;
        public int     Year     { get; set; }
        public decimal Revenue  { get; set; }
        public decimal Expenses { get; set; }
    }

    public class RevenueExpensesResponse
    {
        public IReadOnlyList<RevenueExpensesItem> Data { get; set; } = [];
    }
}
