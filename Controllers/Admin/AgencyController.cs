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
    [Route("api/admin/agencies")]
    public class AgencyController(
        IAgencyService _agencyService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAgencies([FromQuery] AgencyListRequest request)
        {
            var agencies = await _agencyService.GetAllAgenciesAsync(request);

            return Ok(agencies);
        }

        [HttpGet("{agencyId:int}")]
        [EnsureAgencyExistsForAdminAttribute]
        public async Task<IActionResult> GetAgencyProfile([FromRoute] int agencyId)
        {
            var agency = await _agencyService.GetAgencyProfileAsync(agencyId);

            return Ok(new AgencyProfileResponse(agency));
        }

        [HttpPatch("{agencyId:int}")]
        [EnsureAgencyExistsForAdminAttribute]
        public async Task<IActionResult> UpdateAgency([FromRoute] int agencyId, [FromBody] UpdateAgencyRequest request)
        {
            await _agencyService.UpdateAgencyAsync(agencyId, request);
            return Ok(new AgencyResponseDto
            {
                Message = Messages.AgencyUpdatedSuccessfully
            });
        }

        [HttpPatch("{agencyId:int}/update-logo")]
        [EnsureAgencyExistsForAdminAttribute]
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
