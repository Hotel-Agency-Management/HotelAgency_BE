using Booking.Constants;
using Booking.DTO;
using Booking.Filters;
using Booking.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Controllers
{
    [ApiController]
    [Authorize(Roles = $"{Roles.AgencyOwner}")]
    [EnsureAgencyExistsForOwnerAttribute]
    [Route("api/agencies")]
    public class AgencyController(
        IAgencyService _agencyService) : ControllerBase
    {
        [HttpGet("me")]
        public async Task<IActionResult> GetAgencyProfile()
        {
            
            var agencyId = FilterHelpers.GetRequiredAgencyId(HttpContext);
            var agency = await _agencyService.GetAgencyProfileAsync(agencyId);

            return Ok(new AgencyProfileResponse(agency));
        }

        [HttpPatch]
        public async Task<IActionResult> UpdateAgency([FromBody] UpdateAgencyRequest request)
        {
            var agencyId = FilterHelpers.GetRequiredAgencyId(HttpContext);
            await _agencyService.UpdateAgencyAsync(agencyId, request);
            return Ok(new AgencyResponseDto
            {
                Message = Messages.AgencyUpdatedSuccessfully
            });
        }

        [HttpPatch("plan")]
        public async Task<IActionResult> ChangePlan([FromBody] ChangePlanRequest request)
        {
            var agencyId = FilterHelpers.GetRequiredAgencyId(HttpContext);
            await _agencyService.ChangePlanAsync(agencyId, request.PlanId);
            return Ok(new AgencyResponseDto
            {
                Message = Messages.PlanChangedSuccessfully
            });
        }

        [HttpPatch("update-logo")]
        public async Task<IActionResult> UpdateAgencyLogo([FromForm] IFormFile file)
        {
            var agencyId = FilterHelpers.GetRequiredAgencyId(HttpContext);
            await _agencyService.UpdateAgencyLogoAsync(agencyId, file);
            return Ok(new AgencyResponseDto
            {
                Message = Messages.LogoUpdatedSuccessfully
            });
        }
    }
}
