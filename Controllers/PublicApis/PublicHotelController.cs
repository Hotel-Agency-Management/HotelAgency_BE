using Booking.DTO;
using Booking.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Controllers.PublicApis
{
    [ApiController]
    [Route("api/public/hotels")]
    public class PublicHotelController(IHotelService _hotelService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] HotelListRequest request)
        {
            var result = await _hotelService.GetAllHotelsAsync(request);
            return Ok(result);
        }
    }
}
