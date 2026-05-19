using Booking.Constants;
using Booking.DTO;
using Booking.Filters;
using Booking.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Controllers
{
    [ApiController]
    [Authorize(Roles = $"{Roles.AgencyOwner}, {Roles.PropertyManager}, {Roles.Accountant}")]
    [Route("api/hotels/{hotelId}")]
    public class PaymentLogController(IPaymentLogService _paymentLogService) : ControllerBase
    {

        [EnsureHotelExistsForOwner]
        [HttpGet("payment-logs/incoming")]
        public async Task<IActionResult> GetIncoming(
            [FromRoute] int hotelId,
            [FromQuery] PaymentLogListRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _paymentLogService.GetHotelLogsAsync(hotelId, true, request);
            return Ok(result);
        }

        [EnsureHotelExistsForOwner]
        [HttpGet("payment-logs/outgoing")]
        public async Task<IActionResult> GetOutgoing(
            [FromRoute] int hotelId,
            [FromQuery] PaymentLogListRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _paymentLogService.GetHotelLogsAsync(hotelId, false, request);
            return Ok(result);
        }

        [EnsureHotelExistsForOwner]
        [HttpGet("payment-logs/{paymentLogId}")]
        public async Task<IActionResult> GetDetails(
            [FromRoute] int hotelId,
            [FromRoute] int paymentLogId)
        {
            var result = await _paymentLogService.GetDetailsAsync(hotelId, paymentLogId);
            return Ok(result);
        }
    }
}
