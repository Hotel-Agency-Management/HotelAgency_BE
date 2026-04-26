using Booking.DTO;
using Booking.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Booking.Constants;
using Booking.Filters;

namespace Booking.Controllers.Admin
{
    [ApiController]
    [Authorize(Roles = $"{Roles.SuperAdmin}")]
    [EnsureAgencyExistsForAdminAttribute]
    [Route("api/admin/agencies/{agencyId}/documents")]
    public class AgencyDocumentController(IAgencyDocumentService _documentService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetDocuments([FromRoute] int agencyId)
        {
            var documents = await _documentService.GetDocumentsByAgencyAsync(agencyId);
            return Ok(documents);
        }

        [HttpPost]
        public async Task<IActionResult> UploadDocument([FromRoute] int agencyId, [FromForm] UploadDocumentDto dto)
        {
            var result = await _documentService.UploadDocumentAsync(agencyId, dto);
            return CreatedAtAction(nameof(GetDocuments), new { agencyId }, result);
        }

        [HttpPut("{documentId}")]
        public async Task<IActionResult> UpdateDocument([FromRoute] int agencyId, int documentId, [FromForm] UpdateDocumentDto dto)
        {
            var result = await _documentService.UpdateDocumentAsync(documentId, dto);
            return Ok(result);
        }
    }
}
