using Booking.DTOs;
using Booking.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Controllers
{
    [ApiController]
    [Route("api/agencies/{agencyId}/documents")]
    [Authorize]
    public class AgencyDocumentController(IAgencyDocumentService _documentService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetDocuments(int agencyId)
        {
            var documents = await _documentService.GetDocumentsByAgencyAsync(agencyId);
            return Ok(documents);
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadDocument(int agencyId, [FromForm] UploadDocumentDto dto)
        {
            var result = await _documentService.UploadDocumentAsync(agencyId, dto);
            return CreatedAtAction(nameof(GetDocuments), new { agencyId }, result);
        }

        [HttpPut("{documentId}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateDocument(int agencyId, int documentId, [FromForm] UpdateDocumentDto dto)
        {
            var result = await _documentService.UpdateDocumentAsync(documentId, dto);
            return Ok(result);
        }
    }
}
