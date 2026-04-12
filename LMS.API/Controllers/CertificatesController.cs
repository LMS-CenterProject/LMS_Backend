using LMS.Application.DTOs;
using LMS.Application.Features.Certificates.Queries.GetMyCertificates;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers
{
    [ApiController]
    [Route("api/certificates")]
    [Authorize(Roles = "Student")]
    public sealed class CertificatesController(ISender sender) : ControllerBase
    {
        /// <summary>
        /// Get all certificates earned by the current student.
        /// </summary>
        [HttpGet("my")]
        [ProducesResponseType(typeof(IEnumerable<CertificateDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMine(CancellationToken ct)
        {
            var result = await sender.Send(new GetMyCertificatesQuery(), ct);

            return result.IsSuccess
                ? Ok(result.Value)
                : BadRequest(result.Error.Description);
        }
    }
}
