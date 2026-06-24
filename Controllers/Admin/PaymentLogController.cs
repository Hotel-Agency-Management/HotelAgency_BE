using Booking.Constants;
using Booking.DTO;
using Booking.Filters;
using Booking.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Controllers.Admin
{
    [ApiController]
    [Authorize(Roles = $"{Roles.SuperAdmin}")]
    [Route("api/admin")]
    public class PaymentLogController(IPaymentLogService _paymentLogService) : ControllerBase
    {
        [EnsureAgencyExistsForAdmin]
        [EnsureHotelExistsForAdmin]
        [HttpGet("agencies/{agencyId}/hotels/{hotelId}/payment-logs")]
        public async Task<IActionResult> GetByHotel(
            [FromRoute] int hotelId,
            [FromQuery] PaymentLogListRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _paymentLogService.GetHotelLogsAsync(hotelId, request);
            return Ok(result);
        }


        [EnsureAgencyExistsForAdmin]
        [EnsureHotelExistsForAdmin]
        [HttpGet("agencies/{agencyId}/hotels/{hotelId}/payment-logs/{paymentLogId}")]
        public async Task<IActionResult> GetDetails(
            [FromRoute] int hotelId,
            [FromRoute] int paymentLogId)
        {
            var result = await _paymentLogService.GetDetailsAsync(hotelId, paymentLogId);
            return Ok(result);
        }
    }
}
