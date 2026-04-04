using Booking.DTO;
using Booking.Interfaces.Services;
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

        [HttpGet("{agencyId}")]
        public async Task<IActionResult> GetAgencyProfile(int agencyId)
        {
            var result = await _agencyService.GetAgencyProfileAsync(agencyId);
            return Ok(new AgencyProfileResponse
            {
                Id = result.Id,
                OwnerId = result.OwnerId,
                Name = result.AgencyName,
                Phone = result.Phone,
                Country = result.Country,
                City = result.City,
                LogoUrl = result.LogoUrl,
                PrimaryColor = result.PrimaryColor,
                SecondaryColor = result.SecondaryColor,
                TertiaryColor = result.TertiaryColor,
                CreatedAt = result.CreatedAt,
                UpdatedAt = result.UpdatedAt ?? DateTime.UtcNow
            });

        }

        [HttpPatch("{agencyId}")]
        public async Task<IActionResult> UpdateAgency(int agencyId, [FromBody] UpdateAgencyRequest request)
        {
            await _agencyService.UpdateAgencyAsync(agencyId, request);
            return Ok(new AgencyResponseDto
            {
                Message = "Agency Updated Successfully"
            });
        }


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
