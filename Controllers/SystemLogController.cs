using Booking.Constants;
using Booking.DTO;
using Booking.Filters;
using Booking.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Controllers
{
    [ApiController]
    public class SystemLogController(ISystemLogService _systemLogService) : ControllerBase
    {
        [HttpGet("api/agencies/logs")]
        [Authorize(Roles = Roles.AgencyOwner)]
        [EnsureAgencyExistsForOwner]
        public async Task<IActionResult> GetAgencyLogs([FromQuery] SystemLogListRequest request)
        {
            var agencyId = FilterHelpers.GetRequiredAgencyId(HttpContext);
            var logs = await _systemLogService.GetAgencyLogsAsync(agencyId, request);
            return Ok(logs);
        }

        [HttpGet("api/hotels/{hotelId:int}/logs")]
        [Authorize(Roles = $"{Roles.AgencyOwner}, {Roles.PropertyManager}")]
        [EnsureHotelAccessForOwnerOrManager]
        public async Task<IActionResult> GetHotelLogs(
            [FromRoute] int hotelId,
            [FromQuery] SystemLogListRequest request)
        {
            request.HotelId = hotelId;

            var logs = await _systemLogService.GetLogsAsync(request);
            return Ok(logs);
        }
    }
}
