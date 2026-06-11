using Booking.Constants;
using Booking.DTO;
using Booking.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Controllers.Admin
{
    [ApiController]
    [Authorize(Roles = Roles.SuperAdmin)]
    [Route("api/admin/system-logs")]
    public class SystemLogController(ISystemLogService _systemLogService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetLogs([FromQuery] SystemLogListRequest request)
        {
            var logs = await _systemLogService.GetLogsAsync(request);
            return Ok(logs);
        }
    }
}
