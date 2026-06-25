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
            var message = $"Ticket \"{ticket.Title}\" passed its deadline and is still open.";

            await notificationService.CreateAsync(new CreateNotificationRequest
            {
                UserId = ticket.AssignedToId,
                Title = "Overdue Ticket",
                Message = message,
                Type = NotificationType.Alert
            });

            if (ticket.CreatedById != ticket.AssignedToId)
            {
                await notificationService.CreateAsync(new CreateNotificationRequest
                {
                    UserId = ticket.CreatedById,
                    Title = "Overdue Ticket",
                    Message = message,
                    Type = NotificationType.Alert
                });
            }

            ticket.OverdueNotificationSent = true;
            await ticketRepository.UpdateAsync(ticket);

            logger.LogInformation(
                "Overdue notification sent for ticket {TicketId} to assignee {AssignedToId} and creator {CreatedById}",
                ticket.Id, ticket.AssignedToId, ticket.CreatedById);
        }
    }
}
