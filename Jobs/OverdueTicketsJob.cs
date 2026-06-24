using Booking.DTO;
using Booking.Enums;
using Booking.Interfaces.Repositories;
using Booking.Interfaces.Services;

namespace Booking.Jobs;

public class OverdueTicketsJob(
    IHousekeepingTicketRepository ticketRepository,
    INotificationService notificationService,
    ILogger<OverdueTicketsJob> logger)
{
    public async Task ExecuteAsync()
    {
        var utcNow = DateTime.UtcNow;
        var tickets = await ticketRepository.GetOverdueUnnotifiedAsync(utcNow);

        foreach (var ticket in tickets)
        {
            await notificationService.CreateAsync(new CreateNotificationRequest
            {
                UserId = ticket.AssignedToId,
                Title = "Overdue Ticket",
                Message = $"Ticket \"{ticket.Title}\" passed its deadline and is still open.",
                Type = NotificationType.Ticket
            });

            ticket.OverdueNotificationSent = true;
            await ticketRepository.UpdateAsync(ticket);

            logger.LogInformation("Overdue notification sent for ticket {TicketId} to user {UserId}",
                ticket.Id, ticket.AssignedToId);
        }
    }
}
