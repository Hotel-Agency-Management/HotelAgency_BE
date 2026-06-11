using Booking.Constants;
using Booking.DTO;
using Booking.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/dashboard")]
    public class AdminDashboardController(
        IPlanService _planService,
        IAgencyService _agencyService) : ControllerBase
    {
        [Authorize(Roles = Roles.SuperAdmin)]
        [HttpGet("latest-agencies")]
        public async Task<IActionResult> GetLatestAgencies()
        {
            var result = await _agencyService.GetAllAgenciesAsync(new AgencyListRequest
            {
                Page      = 1,
                Limit     = 10,
                SortOrder = "Newest"
            });
            return Ok(result);
        }

        [Authorize(Roles = Roles.SuperAdmin)]
        [HttpGet("agency-growth")]
        public async Task<IActionResult> GetAgencyGrowth()
        {
            var result = await _agencyService.GetMonthlyGrowthAsync();
            return Ok(result);
        }

        [Authorize(Roles = Roles.SuperAdmin)]
        [HttpGet("revenue")]
        public async Task<IActionResult> GetRevenue()
        {
            var result = await _planService.GetRevenueOverviewAsync();
            return Ok(result);
        }

        [Authorize(Roles = Roles.SuperAdmin)]
        [HttpGet("subscription-distribution")]
        public async Task<IActionResult> GetSubscriptionDistribution()
        {
            var result = await _planService.GetSubscriptionDistributionAsync();
            return Ok(result);
        }

        [Authorize(Roles = Roles.SuperAdmin)]
        [HttpGet("agency-status")]
        public async Task<IActionResult> GetAgencyStatus()
        {
            var counts = await _agencyService.GetAgencyStatusCountsAsync();

            return Ok(new AgencyStatusChartResponse
            {
                Total = counts.Total,
                Breakdown =
                [
                    new AgencyStatusSlice { Status = "Active",     Count = counts.Active },
                    new AgencyStatusSlice { Status = "Pending",    Count = counts.Pending },
                    new AgencyStatusSlice { Status = "Rejected",   Count = counts.Rejected },
                    new AgencyStatusSlice { Status = "InComplete", Count = counts.InComplete },
                ]
            });
        }

        [Authorize(Roles = Roles.SuperAdmin)]
        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary()
        {
            var counts       = await _agencyService.GetAgencyStatusCountsAsync();
            var totalRevenue = await _planService.GetTotalSubscriptionRevenueAsync();

            return Ok(new DashboardSummaryResponse
            {
                TotalAgencies       = counts.Total,
                PendingApprovals    = counts.Pending,
                ActiveSubscriptions = counts.Active,
                TotalRevenue        = totalRevenue
            });
        }
    }
}
