using Booking.DTO;
using Booking.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Booking.Constants;
using Booking.Filters;
using Booking.Models;
using Microsoft.AspNetCore.Identity;


namespace Booking.Controllers
{
    [ApiController]
    [Authorize(Roles = $"{Roles.AgencyOwner}")]
    [EnsureAgencyExistsForOwnerAttribute]
    [Route("api/documents")]
    public class AgencyDocumentController(
        IAgencyDocumentService _documentService,
        UserManager<ApplicationUser> _userManager) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetDocuments()
        {
            var agencyOwner = await _userManager.GetUserAsync(User);
            var documents = await _documentService.GetDocumentsByAgencyAsync(agencyOwner!.AgencyId!.Value);
            return Ok(documents);
        }

        [HttpPost]
        public async Task<IActionResult> UploadDocument([FromForm] UploadDocumentDto dto)
        {
            var agencyOwner = await _userManager.GetUserAsync(User);
            var result = await _documentService.UploadDocumentAsync(agencyOwner!.AgencyId!.Value, dto);
            return CreatedAtAction(nameof(GetDocuments), new { agencyOwner!.AgencyId!.Value }, result);
        }

        [HttpPut("{documentId}")]
        public async Task<IActionResult> UpdateDocument(int documentId, [FromForm] UpdateDocumentDto dto)
        {
            var result = await _documentService.UpdateDocumentAsync(documentId, dto);
            return Ok(result);
        }
    }
}
