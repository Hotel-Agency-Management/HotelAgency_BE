namespace Booking.Models
{
    public class AgencyDocument
    {
        public int Id { get; set; }
        public string DocumentType { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public int AgencyId { get; set; }
        public Agency Agency { get; set; } = null!;
        public DateTime UploadedAt {get;set;}
    }
}
