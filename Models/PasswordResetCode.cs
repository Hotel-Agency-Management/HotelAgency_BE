namespace Booking.Models
{
    public class PasswordResetCode
    {
        public int Id { get; set; }
        public required int UserId { get; set; }
        public required string Code { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsUsed { get; set; }
        public ApplicationUser User { get; set; } = null!;
    }
}
