using Booking.Constants;
using Booking.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/dashboard")]
    public class AdminDashboardController(IPlanService _planService) : ControllerBase
    {
        [Authorize(Roles = Roles.SuperAdmin)]
        [HttpGet("revenue")]
        public async Task<IActionResult> GetRevenue()
        {
            var result = await _planService.GetRevenueOverviewAsync();
            return Ok(result);
        }
    }
}
