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
        [HttpGet("payment-logs")]
        public async Task<IActionResult> GetAll([FromQuery] PaymentLogListRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _paymentLogService.GetAllAsync(request);
            return Ok(result);
        }

        [EnsureAgencyExistsForAdmin]
        [EnsureHotelExistsForAdmin]
        [HttpGet("agencies/{agencyId}/hotels/{hotelId}/payment-logs")]
        public async Task<IActionResult> GetByHotel(
            [FromRoute] int hotelId,
            [FromQuery] PaymentLogListRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _paymentLogService.GetByHotelIdAsync(hotelId, request);
            return Ok(result);
        }

        [EnsureAgencyExistsForAdmin]
        [EnsureHotelExistsForAdmin]
        [HttpGet("agencies/{agencyId}/hotels/{hotelId}/payment-logs/incoming")]
        public async Task<IActionResult> GetIncoming(
            [FromRoute] int hotelId,
            [FromQuery] PaymentLogListRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _paymentLogService.GetIncomingByHotelIdAsync(hotelId, request);
            return Ok(result);
        }

        [EnsureAgencyExistsForAdmin]
        [EnsureHotelExistsForAdmin]
        [HttpGet("agencies/{agencyId}/hotels/{hotelId}/payment-logs/outgoing")]
        public async Task<IActionResult> GetOutgoing(
            [FromRoute] int hotelId,
            [FromQuery] PaymentLogListRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _paymentLogService.GetOutgoingByHotelIdAsync(hotelId, request);
            return Ok(result);
        }
    }
}
