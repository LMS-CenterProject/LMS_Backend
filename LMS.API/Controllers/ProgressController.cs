using LMS.Application.DTOs;
using LMS.Application.Features.Progress.Commands.UpdateLessonProgress;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers
{
    [ApiController]
    [Route("api/progress")]
    [Authorize(Roles = "Student")]
    public sealed class ProgressController(ISender sender) : ControllerBase
    {
        /// <summary>
        /// Update watch progress for a lesson.
        /// Set isCompleted = true to mark the lesson as done.
        /// If all lessons are complete, enrollment auto-completes and certificate is issued.
        /// </summary>
        [HttpPatch("lessons/{lessonId:guid}")]
        [ProducesResponseType(typeof(UpdateProgressResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateProgress(
            Guid lessonId,
            [FromBody] ProgressRequest request,
            CancellationToken ct)
        {
            var result = await sender.Send(
                new UpdateLessonProgressCommand(
                    lessonId,
                    request.WatchedSeconds,
                    request.IsCompleted), ct);

            if (result.IsFailure)
            {
                var statusCode = result.Error.Code switch
                {
                    "Lesson.NotFound" => StatusCodes.Status404NotFound,
                    "LessonProgress.NotEnrolled" => StatusCodes.Status403Forbidden,
                    _ => StatusCodes.Status400BadRequest
                };

                return Problem(
                    detail: result.Error.Description,
                    title: result.Error.Code,
                    statusCode: statusCode);
            }

            return Ok(result.Value);
        }
    }

    public sealed record ProgressRequest(int WatchedSeconds, bool IsCompleted);
}
