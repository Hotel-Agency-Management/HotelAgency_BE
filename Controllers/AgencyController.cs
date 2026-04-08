using Booking.Constants;
using Booking.DTO;
using Booking.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Booking.Models;
using Booking.Exceptions;

namespace Booking.Controllers
{
    [ApiController]
    [Route("api/agencies")]
    public class AgencyController(
        IAgencyService _agencyService,
        UserManager<ApplicationUser> _userManager) : ControllerBase
    {

        [Authorize(Roles = $"{Roles.AgencyOwner}")]
        [HttpGet("me")]
        public async Task<IActionResult> GetAgencyProfile()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                throw new UserNotFoundException("Authenticated user was not found.");

            if (!user.AgencyId.HasValue)
                throw new AgencyNotAssignedException();

            var result = await _agencyService.GetAgencyProfileAsync(user.AgencyId.Value);

            return Ok(new AgencyProfileResponse(result));
        }

        [Authorize(Roles = $"{Roles.AgencyOwner}")]
        [HttpPatch]
        public async Task<IActionResult> UpdateAgency([FromBody] UpdateAgencyRequest request)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                throw new UserNotFoundException("Authenticated user was not found.");

            if (!user.AgencyId.HasValue)
                throw new AgencyNotAssignedException();

            await _agencyService.UpdateAgencyAsync(user.AgencyId.Value, request);
            return Ok(new AgencyResponseDto
            {
                Message = "Agency Updated Successfully"
            });
        }

        [Authorize(Roles = $"{Roles.SuperAdmin},{Roles.AgencyOwner}")]
        [HttpPatch("update-logo")]
        public async Task<IActionResult> UpdateAgencyLogo([FromForm] IFormFile file)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                throw new UserNotFoundException("Authenticated user was not found.");

            if (!user.AgencyId.HasValue)
                throw new AgencyNotAssignedException();

            var result = await _agencyService.UpdateAgencyLogoAsync(user.AgencyId.Value, file);
            return Ok(new AgencyResponseDto
            {
                Message = "Logo Updated Successfully"

            });
        }
    }
}
