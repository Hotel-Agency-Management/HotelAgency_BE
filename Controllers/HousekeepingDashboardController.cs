using Booking.Constants;
using Booking.Filters;
using Booking.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Controllers
{
    [ApiController]
    [Authorize(Roles = $"{Roles.AgencyOwner},{Roles.PropertyManager},{Roles.HousekeepingManager}")]
    [EnsureHotelExistsForOwnerAttribute]
    [Route("api/hotels/{hotelId}/housekeeping/dashboard")]
    public class HousekeepingDashboardController(
        IHousekeepingTicketService _ticketService) : ControllerBase
    {
        [HttpGet("ticket-status-distribution")]
        public async Task<IActionResult> GetTicketStatusDistribution([FromRoute] int hotelId)
        {
            var result = await _ticketService.GetTicketStatusDistributionAsync(hotelId);
            return Ok(result);
        }

        [HttpGet("ticket-completion-rate")]
        public async Task<IActionResult> GetTicketCompletionRate([FromRoute] int hotelId)
        {
            var result = await _ticketService.GetHotelTicketCompletionRateAsync(hotelId);
            return Ok(result);
        }

        [HttpGet("kpi")]
        public async Task<IActionResult> GetKpi([FromRoute] int hotelId)
        {
            var result = await _ticketService.GetHousekeepingKpiAsync(hotelId);
            return Ok(result);
        }

        [HttpGet("ticket-priorities")]
        public async Task<IActionResult> GetTicketPriorityDistribution([FromRoute] int hotelId)
        {
            var result = await _ticketService.GetTicketPriorityDistributionAsync(hotelId);
            return Ok(result);
        }

        [HttpGet("ticket-types")]
        public async Task<IActionResult> GetTicketTypeDistribution([FromRoute] int hotelId)
        {
            var result = await _ticketService.GetTicketTypeDistributionAsync(hotelId);
            return Ok(result);
        }

        [HttpGet("open-tickets-over-time")]
        public async Task<IActionResult> GetOpenTicketsOverTime(
            [FromRoute] int hotelId,
            [FromQuery] string granularity = DashboardConstants.GroupByDay)
        {
            var result = await _ticketService.GetOpenTicketsOverTimeAsync(hotelId, granularity);
            return Ok(result);
        }
    }
}
