using Booking.Constants;
using Booking.DTO;
using Booking.Filters;
using Booking.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Controllers.Admin
{
    [ApiController]
    [Authorize(Roles = $"{Roles.SuperAdmin}")]
    [EnsureAgencyExistsForAdminAttribute]
    [Route("api/admin/agencies/{agencyId}")]
    public class AgencyController(
        IAgencyService _agencyService) : ControllerBase
    {
        [HttpGet()]
        public async Task<IActionResult> GetAgencyProfile([FromRoute] int agencyId)
        {
            var agency = await _agencyService.GetAgencyProfileAsync(agencyId);

            return Ok(new AgencyProfileResponse(agency));
        }

        [HttpPatch]
        public async Task<IActionResult> UpdateAgency([FromRoute] int agencyId, [FromBody] UpdateAgencyRequest request)
        {
            await _agencyService.UpdateAgencyAsync(agencyId, request);
            return Ok(new AgencyResponseDto
            {
                Message = Messages.AgencyUpdatedSuccessfully
            });
        }

        [HttpPatch("update-logo")]
        public async Task<IActionResult> UpdateAgencyLogo([FromRoute] int agencyId, [FromForm] IFormFile file)
        {
            await _agencyService.UpdateAgencyLogoAsync(agencyId, file);
            return Ok(new AgencyResponseDto
            {
                Message = Messages.LogoUpdatedSuccessfully
            });
        }
    }
}
