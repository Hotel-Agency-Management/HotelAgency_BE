using Booking.DTO;
using Booking.Exceptions;
using Booking.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Booking.Constants;


namespace Booking.Controllers.Admin
{
    [ApiController]
    [Route("api/[controller]")]

    public class PlansController(IPlanService _planService) : ControllerBase
    {

        [Authorize(Roles = $"{Roles.SuperAdmin}")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePlanDto dto)
        {
            var created = await _planService.CreatePlanAsync(dto);
            return Ok(created);
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
    }
}
