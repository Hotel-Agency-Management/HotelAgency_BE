using Booking.DTO;
using Booking.Exceptions;
using Booking.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Booking.Constants;


namespace Booking.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class PlansController(IPlanService _planService) : ControllerBase
    {
        [Authorize(Roles = $"{Roles.SuperAdmin}, {Roles.AgencyOwner}")]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] bool includeInactive = false)
        {
            var plans = await _planService.GetPlansAsync(includeInactive);
            return Ok(plans);
        }

        [Authorize(Roles = $"{Roles.SuperAdmin}, {Roles.AgencyOwner}")]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var plan = await _planService.GetPlanByIdAsync(id);
            return Ok(plan);
        }

        [Authorize(Roles = $"{Roles.SuperAdmin}")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePlanDto dto)
        {
            var created = await _planService.CreatePlanAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [Authorize(Roles = $"{Roles.SuperAdmin}")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdatePlanDto dto)
        {
            var updated = await _planService.UpdatePlanAsync(id, dto);
            return Ok(updated);
        }

        [Authorize(Roles = $"{Roles.SuperAdmin}")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _planService.DeletePlanAsync(id);
            return NoContent();
        }

        [Authorize(Roles = $"{Roles.SuperAdmin}")]
        [HttpPatch("{id:int}/toggle-status")]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            await _planService.TogglePlanStatusAsync(id);
            return NoContent();
        }
    }
}
