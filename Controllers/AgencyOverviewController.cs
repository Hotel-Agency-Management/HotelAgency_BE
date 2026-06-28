using Booking.Constants;
using Booking.DTO;
using Booking.Models;
using Booking.Filters;
using Booking.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;


namespace Booking.Controllers
{
    [ApiController]
    [Authorize(Roles = $"{Roles.AgencyOwner}, {Roles.SuperAdmin}")]
    [EnsureAgencyExistsForOwnerAttribute]
    [Route("api/agencies/{agencyId}/overview")]
    public class AgencyOverviewController(
        IReservationService _reservationService,
        IPaymentLogService _paymentLogService,
        UserManager<ApplicationUser> _userManager) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetOverview(int agencyId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized(Messages.Unauthorized);

            var bookingStats = await _reservationService.GetAgencyStatsAsync(user.AgencyId!.Value);
            var revenueStats = await _paymentLogService.GetAgencyRevenueStatsAsync(user.AgencyId!.Value);

            return Ok(new AgencyOverviewResponse
            {
                TotalRevenue        = revenueStats,
                TotalBookings       = bookingStats.TotalBookings,
                PendingReservations = bookingStats.PendingCount,
                AverageBookingValue = bookingStats.AverageBookingValue
            });
        }

        [HttpGet("revenue-trend")]
        public async Task<IActionResult> GetRevenueTrend(int agencyId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized(Messages.Unauthorized);

            var trend = await _paymentLogService.GetAgencyRevenueTrendAsync(user.AgencyId!.Value);
            return Ok(trend);
        }

        [HttpGet("status-distribution")]
        public async Task<IActionResult> GetStatusDistribution(int agencyId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized(Messages.Unauthorized);

            var distribution = await _reservationService.GetAgencyStatusDistributionAsync(user.AgencyId!.Value);
            return Ok(distribution);
        }

        [HttpGet("booking-distribution")]
        public async Task<IActionResult> GetBookingDistribution(int agencyId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized(Messages.Unauthorized);

            var distribution = await _reservationService.GetAgencyBookingTypeDistributionAsync(user.AgencyId!.Value);
            return Ok(distribution);
        }

        [HttpGet("revenue-per-hotel")]
        public async Task<IActionResult> GetRevenuePerHotel(int agencyId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized(Messages.Unauthorized);

            var result = await _paymentLogService.GetAgencyRevenuePerHotelAsync(user.AgencyId!.Value);
            return Ok(result);
        }

        [HttpGet("profit-expenses")]
        public async Task<IActionResult> GetProfitExpenses(int agencyId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized(Messages.Unauthorized);

            var resolvedId = user.AgencyId!.Value;
            var profit   = await _paymentLogService.GetAgencyMonthlyProfitAsync(resolvedId);
            var expenses = await _paymentLogService.GetAgencyMonthlyExpensesAsync(resolvedId);

            var result = profit.Select((p, i) => new MonthlyProfitExpensesItem
            {
                Month    = p.Month,
                Year     = p.Year,
                Profit   = p.Revenue,
                Expenses = expenses[i].Revenue
            }).ToList();

            return Ok(result);
        }

        [HttpGet("reservations-by-room-type")]
        public async Task<IActionResult> GetReservationsByRoomType(int agencyId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized(Messages.Unauthorized);

            var result = await _reservationService.GetAgencyReservationsByRoomTypeAsync(user.AgencyId!.Value);
            return Ok(result);
        }
    }
}
