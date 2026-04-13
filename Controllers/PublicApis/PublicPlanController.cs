using Booking.DTO;
using Booking.Exceptions;
using Booking.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;


namespace Booking.Controllers
{
    [ApiController]
    [Route("api/public/plans")]

    public class PublicPlansController(IPlanService _planService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] bool includeInactive = false)
        {
            var plans = await _planService.GetPlansAsync(includeInactive);
            return Ok(plans);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var plan = await _planService.GetPlanByIdAsync(id);
            return Ok(plan);
        }
    }
}
