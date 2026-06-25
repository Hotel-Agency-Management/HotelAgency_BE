using Booking.DTO;
using Booking.Enums;
using Booking.Interfaces.Repositories;
using Booking.Interfaces.Services;

namespace Booking.Jobs;

public class CheckInReminderJob(
    IReservationRepository reservationRepository,
    INotificationService notificationService,
    ILogger<CheckInReminderJob> logger)
{
    public async Task ExecuteAsync()
    {
        var tomorrow = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
        var reservations = await reservationRepository.GetTomorrowCheckInsWithCustomerAsync(tomorrow);

        foreach (var reservation in reservations)
        {
            await notificationService.CreateAsync(new CreateNotificationRequest
            {
                UserId = reservation.CustomerId!.Value,
                Title = "Check-in Reminder",
                Message = $"Your reservation #{reservation.ReservationNumber} is scheduled for check-in tomorrow, {reservation.CheckInDate:MMMM d, yyyy}.",
                Type = NotificationType.Reservation
            });

            reservation.CheckInReminderSent = true;
            await reservationRepository.UpdateAsync(reservation);

            logger.LogInformation("Check-in reminder sent for reservation {Number} to customer {CustomerId}",
                reservation.ReservationNumber, reservation.CustomerId);
        }
    }
}
