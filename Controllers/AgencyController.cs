using Booking.Constants;
using Booking.DTO;
using Booking.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Controllers
{
    [ApiController]
    [Route("api/agencies")]
    public class AgencyController(IAgencyService _agencyService) : ControllerBase
    {

        [HttpPost]
        public async Task<IActionResult> CreateAgency([FromBody] CreateAgencyRequest request)
        {
            var result = await _agencyService.CreateAgencyAsync(request);
            return Ok(new CreateAgencyResponse
            {
                Id = result.Id,
                Message = "Agency Created Successfully"

            });
        }

        [Authorize(Roles = $"{Roles.SuperAdmin},{Roles.AgencyOwner}")]
        [HttpGet("{agencyId}")]
        public async Task<IActionResult> GetAgencyProfile(int agencyId)
        {
            var result = await _agencyService.GetAgencyProfileAsync(agencyId);
            return Ok(new AgencyProfileResponse(result));

        }

        [Authorize(Roles = $"{Roles.SuperAdmin},{Roles.AgencyOwner}")]
        [HttpPatch("{agencyId}")]
        public async Task<IActionResult> UpdateAgency(int agencyId, [FromBody] UpdateAgencyRequest request)
        {
            await _agencyService.UpdateAgencyAsync(agencyId, request);
            return Ok(new AgencyResponseDto
            {
                Message = "Agency Updated Successfully"
            });
        }

        [Authorize(Roles = $"{Roles.SuperAdmin},{Roles.AgencyOwner}")]
        [HttpPatch("{agencyId}/update-logo")]
        public async Task<IActionResult> UpdateAgencyLogo([FromRoute] int agencyId, [FromForm] IFormFile file)
        {
            var result = await _agencyService.UpdateAgencyLogoAsync(agencyId, file);
            return Ok(new AgencyResponseDto
            {
                Message = "Logo Updated Successfully"

            });
        }
    }
}
