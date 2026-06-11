using Booking.Constants;
using Booking.Filters;
using Booking.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Controllers
{
    [ApiController]
    [Authorize(Roles = $"{Roles.AgencyOwner}, {Roles.PropertyManager}, {Roles.Accountant}")]
    [Route("api/hotels/{hotelId}")]
    public class AccountantController(IPaymentLogService _paymentLogService) : ControllerBase
    {
        [EnsureHotelExistsForOwner]
        [HttpGet("cash-flow")]
        public async Task<IActionResult> GetCashFlow([FromRoute] int hotelId)
        {
            var result = await _paymentLogService.GetHotelCashFlowAsync(hotelId);
            return Ok(result);
        }

        [EnsureHotelExistsForOwner]
        [HttpGet("revenue-by-type")]
        public async Task<IActionResult> GetRevenueByType([FromRoute] int hotelId)
        {
            var result = await _paymentLogService.GetHotelRevenueByTypeAsync(hotelId);
            return Ok(result);
        }
    }
}
