namespace Booking.DTO
{
    public class TicketStatusSlice
    {
        public string Status { get; set; } = string.Empty;
        public int Count { get; set; }
        public decimal Percentage { get; set; }
    }

    public class TicketStatusDistributionResponse
    {
        public int Total { get; set; }
        public List<TicketStatusSlice> Items { get; set; } = new();
    }

    public class KpiCount
    {
        public int Count { get; set; }
    }

    public class CompletionRateKpi
    {
        public int Done { get; set; }
        public int Total { get; set; }
        public decimal Rate { get; set; }
    }

    public class HousekeepingKpiResponse
    {
        public KpiCount ActiveTickets { get; set; } = new();
        public KpiCount OverdueTickets { get; set; } = new();
        public KpiCount HighPriorityTickets { get; set; } = new();
        public CompletionRateKpi CompletionRate { get; set; } = new();
    }

    public class TicketTypeSlice
    {
        public string Type { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class TicketTypeDistributionResponse
    {
        public int Total { get; set; }
        public List<TicketTypeSlice> Data { get; set; } = new();
    }

    public class TicketPrioritySlice
    {
        public string Priority { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class TicketPriorityDistributionResponse
    {
        public int Total { get; set; }
        public List<TicketPrioritySlice> Data { get; set; } = new();
    }

    public class OpenTicketsTrendItem
    {
        public string Date { get; set; } = string.Empty;
        public int Open { get; set; }
    }

    public class OpenTicketsOverTimeResponse
    {
        public string Granularity { get; set; } = string.Empty;
        public string From { get; set; } = string.Empty;
        public string To { get; set; } = string.Empty;
        public List<OpenTicketsTrendItem> Series { get; set; } = new();
    }
}
