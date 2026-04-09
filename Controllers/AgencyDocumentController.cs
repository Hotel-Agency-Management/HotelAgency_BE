using Booking.DTOs;
using Booking.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Booking.Constants;

namespace Booking.Controllers
{
    [ApiController]
    [Authorize(Roles = $"{Roles.AgencyOwner}, {Roles.SuperAdmin}")]
    [Route("api/agencies/{agencyId}/documents")]
    public class AgencyDocumentController(IAgencyDocumentService _documentService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetDocuments(int agencyId)
        {
            var documents = await _documentService.GetDocumentsByAgencyAsync(agencyId);
            return Ok(documents);
        }

        [HttpPost]
        public async Task<IActionResult> UploadDocument(int agencyId, [FromForm] UploadDocumentDto dto)
        {
            var result = await _documentService.UploadDocumentAsync(agencyId, dto);
            return CreatedAtAction(nameof(GetDocuments), new { agencyId }, result);
        }

        [HttpPut("{documentId}")]
        public async Task<IActionResult> UpdateDocument(int agencyId, int documentId, [FromForm] UpdateDocumentDto dto)
        {
            var result = await _documentService.UpdateDocumentAsync(documentId, dto);
            return Ok(result);
        }
    }
}
