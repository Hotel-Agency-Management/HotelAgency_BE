using System.ComponentModel.DataAnnotations.Schema;

namespace Booking.Models
{
    public class ReservationRoom
    {
        public int ReservationId { get; set; }
        public int RoomId { get; set; }

        [ForeignKey(nameof(ReservationId))] public Reservation? Reservation { get; set; }
        [ForeignKey(nameof(RoomId))]        public Room? Room { get; set; }
    }
}
