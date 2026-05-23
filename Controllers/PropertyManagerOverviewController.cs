using Booking.Constants;
using Booking.DTO;
using Booking.Filters;
using Booking.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Controllers
{
    [ApiController]
    [Authorize(Roles = $"{Roles.AgencyOwner},{Roles.PropertyManager}")]
    [EnsureHotelExistsForOwnerAttribute]
    [Route("api/hotels/{hotelId}/overview")]
    public class PropertyManagerOverviewController(
        IRoomService _roomService,
        IReservationService _reservationService) : ControllerBase
    {
        [HttpGet("room-status-distribution")]
        public async Task<IActionResult> GetRoomStatusDistribution([FromRoute] int hotelId)
        {
            var result = await _roomService.GetRoomStatusDistributionAsync(hotelId);
            return Ok(result);
        }

        [HttpGet("cards")]
        public async Task<IActionResult> GetOverviewCards([FromRoute] int hotelId)
        {
            var result = await _reservationService.GetHotelOverviewCardsAsync(hotelId);
            return Ok(result);
        }

        [HttpGet("reservation-status-distribution")]
        public async Task<IActionResult> GetReservationStatusDistribution([FromRoute] int hotelId)
        {
            var result = await _reservationService.GetHotelReservationStatusDistributionAsync(hotelId);
            return Ok(result);
        }

        [HttpGet("booking-types-distribution")]
        public async Task<IActionResult> GetBookingTypesDistribution([FromRoute] int hotelId)
        {
            var result = await _reservationService.GetHotelBookingTypeDistributionAsync(hotelId);
            return Ok(result);
        }

        [HttpGet("revenue-trend")]
        public async Task<IActionResult> GetRevenueTrend(
            [FromRoute] int hotelId,
            [FromQuery] string groupBy = "monthly")
        {
            var result = await _reservationService.GetHotelRevenueTrendAsync(hotelId, groupBy);
            return Ok(result);
        }
    }
}
