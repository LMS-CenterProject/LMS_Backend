using LMS.Application.Features.Section.Command.CreateSection;
using LMS.Application.Features.Section.Command.DeleteSection;
using LMS.Application.Features.Section.Command.UpdateSection;
using LMS.Application.Features.Section.Query.GetSectionByCourse;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SectionsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SectionsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/Sections/Course/{courseId}
        [HttpGet("Course/{courseId}")]
        [Authorize(Policy = "ReadCourse")]
        public async Task<IActionResult> GetSectionsByCourse(Guid courseId)
        {
            var result = await _mediator.Send(new GetSectionsByCourseQuery(courseId));
            return Ok(result);
        }

        // POST: api/Sections
        [HttpPost]
        [Authorize(Policy = "ManageCourses")]
        public async Task<IActionResult> CreateSection([FromBody] AddSectionCommand command)
        {
            var sectionId = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetSectionsByCourse), new { courseId = command.CourseId }, sectionId);
        }

        // PUT: api/Sections/{id}
        [HttpPut("{id}")]
        [Authorize(Policy = "ManageCourses")]
        public async Task<IActionResult> UpdateSection(Guid id, [FromBody] UpdateSectionCommand command)
        {
            var updatedCommand = command with { SectionId = id };
            await _mediator.Send(updatedCommand);
            return NoContent();
        }

        // DELETE: api/Sections/{id}
        [HttpDelete("{id}")]
        [Authorize(Policy = "ManageCourses")]
        public async Task<IActionResult> DeleteSection(Guid id)
        {
            await _mediator.Send(new DeleteSectionCommand(id));
            return NoContent();
        }
    }
}