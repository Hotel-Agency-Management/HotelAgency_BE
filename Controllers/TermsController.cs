using Booking.Constants;
using Booking.DTO;
using Booking.Filters;
using Booking.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Controllers
{
    [ApiController]
    [EnsureAgencyExistsForOwner]
    [EnsureHotelExistsForOwnerAttribute]
    [Route("api/hotels/{hotelId}/terms")]
    public class TermsController(ITermsAndConditionsService _termsService) : ControllerBase
    {
        [Authorize(Roles = $"{Roles.AgencyOwner},{Roles.PropertyManager}, {Roles.FrontDeskManager}, {Roles.FrontDeskStaff}, {Roles.Customer}")]
        [HttpGet]
        public async Task<IActionResult> GetByHotelId([FromRoute] int hotelId)
        {
            var terms = await _termsService.GetTermsByHotelIdAsync(hotelId);
            return Ok(terms);
        }

        [Authorize(Roles = $"{Roles.AgencyOwner},{Roles.PropertyManager}, {Roles.FrontDeskManager}, {Roles.FrontDeskStaff}, {Roles.Customer}")]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(
            [FromRoute] int hotelId,
            [FromRoute] int id)
        {
            var terms = await _termsService.GetTermsByIdAsync(id);
            return Ok(terms);
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            [FromRoute] int hotelId,
            [FromBody] CreateTermsRequest dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _termsService.CreateTermsAsync(hotelId, dto);
            return Created(string.Empty, created);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            [FromRoute] int hotelId,
            [FromRoute] int id,
            [FromBody] UpdateTermsRequest dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _termsService.UpdateTermsAsync(id, dto);
            return Ok(updated);
        }
    }
}
