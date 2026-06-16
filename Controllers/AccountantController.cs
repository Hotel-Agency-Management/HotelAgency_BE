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

        [EnsureHotelExistsForOwner]
        [HttpGet("balance-trend")]
        public async Task<IActionResult> GetBalanceTrend([FromRoute] int hotelId)
        {
            var result = await _paymentLogService.GetHotelBalanceTrendAsync(hotelId);
            return Ok(result);
        }

        [EnsureHotelExistsForOwner]
        [HttpGet("refund-impact")]
        public async Task<IActionResult> GetRefundImpact([FromRoute] int hotelId)
        {
            var result = await _paymentLogService.GetHotelRefundImpactAsync(hotelId);
            return Ok(result);
        }

        [EnsureHotelExistsForOwner]
        [HttpGet("revenue-growth")]
        public async Task<IActionResult> GetRevenueGrowth(
            [FromRoute] int hotelId,
            [FromQuery] int? month,
            [FromQuery] int? year)
        {
            var now = DateTime.UtcNow;
            int resolvedMonth = month ?? now.Month;
            int resolvedYear  = year  ?? now.Year;

            if (resolvedMonth < 1 || resolvedMonth > 12)
                return BadRequest("month must be between 1 and 12.");

            var result = await _paymentLogService.GetHotelRevenueGrowthAsync(hotelId, resolvedMonth, resolvedYear);
            return Ok(result);
        }

        [EnsureHotelExistsForOwner]
        [HttpGet("financial-summary")]
        public async Task<IActionResult> GetFinancialSummary([FromRoute] int hotelId)
        {
            var result = await _paymentLogService.GetHotelFinancialSummaryAsync(hotelId);
            return Ok(result);
        }

        [EnsureHotelExistsForOwner]
        [HttpGet("revenue-expenses")]
        public async Task<IActionResult> GetRevenueExpenses([FromRoute] int hotelId)
        {
            var result = await _paymentLogService.GetHotelRevenueExpensesAsync(hotelId);
            return Ok(result);
        }
    }
}
