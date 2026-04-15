using LMS.Application.Features.Lesson.Command.AddLesson;
using LMS.Application.Features.Lesson.Command.DeleteLesson;
using LMS.Application.Features.Lesson.Command.UpdateLesson;
using LMS.Application.Features.Lesson.Commands.ToggleFreePreview;
using LMS.Application.Features.Lesson.Query.GetLessonBySection;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LessonsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public LessonsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/Lessons/Section/{sectionId}
        [HttpGet("Section/{sectionId}")]
        [Authorize(Policy = "ReadCourse")]
        public async Task<IActionResult> GetLessonsBySection(Guid sectionId)
        {
            var result = await _mediator.Send(new GetLessonsBySectionQuery(sectionId));
            return Ok(result);
        }

        // POST: api/Lessons
        [HttpPost]
        [Authorize(Policy = "CreateCourse")]
        public async Task<IActionResult> CreateLesson([FromBody] AddLessonCommand command)
        {
            var lessonId = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetLessonsBySection), new { sectionId = command.SectionId }, lessonId);
        }

        // PUT: api/Lessons/{id}
        [HttpPut("{id}")]
        [Authorize(Policy = "CreateCourse")]
        public async Task<IActionResult> UpdateLesson(Guid id, [FromBody] UpdateLessonCommand command)
        {
            var updatedCommand = command with { LessonId = id };
            await _mediator.Send(updatedCommand);
            return NoContent();
        }

        // PUT: api/Lessons/{id}/toggle-preview
        [HttpPut("{id}/toggle-preview")]
        [Authorize(Policy = "CreateCourse")]
        public async Task<IActionResult> ToggleFreePreview(Guid id)
        {
            await _mediator.Send(new ToggleFreePreviewCommand(id));
            return NoContent();
        }

        // DELETE: api/Lessons/{id}
        [HttpDelete("{id}")]
        [Authorize(Policy = "CreateCourse")]
        public async Task<IActionResult> DeleteLesson(Guid id)
        {
            await _mediator.Send(new DeleteLessonCommand(id));
            return NoContent();
        }
    }
}