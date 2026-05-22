using LMS.Application.DTOs;
using LMS.Application.Features.Enrollments.Commands.EnrollStudent;
using LMS.Application.Features.Enrollments.Queries.GetCourseEnrollments;
using LMS.Application.Features.Enrollments.Queries.GetEnrollmentById;
using LMS.Application.Features.Enrollments.Queries.GetMyEnrollments;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers
{
    [ApiController]
    [Route("api/enrollments")]
    [Authorize]
    public sealed class EnrollmentsController(ISender sender) : ControllerBase
    {
        /// <summary>
        /// Enroll the current student in a course.
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Student")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Enroll(
            [FromBody] EnrollStudentCommand command,
            CancellationToken ct)
        {
            var result = await sender.Send(command, ct);

            if (result.IsFailure)
            {
                var statusCode = result.Error.Code switch
                {
                    "Course.NotFound" => StatusCodes.Status404NotFound,
                    "Enrollment.AlreadyExists" => StatusCodes.Status409Conflict,
                    _ => StatusCodes.Status400BadRequest
                };

                return Problem(
                    detail: result.Error.Description,
                    title: result.Error.Code,
                    statusCode: statusCode);
            }

            return CreatedAtAction(
                nameof(GetById),
                new { id = result.Value },
                new { enrollmentId = result.Value });
        }

        /// <summary>
        /// Get all enrollments for the current student with progress.
        /// </summary>
        [HttpGet("myEnrollments")]
        [Authorize(Roles = "Student")]
        [ProducesResponseType(typeof(IEnumerable<EnrollmentDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMine(CancellationToken ct)
        {
            var result = await sender.Send(new GetMyEnrollmentsQuery(), ct);
            return result.IsSuccess
                ? Ok(result.Value)
                : BadRequest(result.Error.Description);
        }

        /// <summary>
        /// Get a single enrollment with full lesson progress breakdown.
        /// </summary>
        [HttpGet("{id:guid}")]
        [Authorize(Roles = "Student")]
        [ProducesResponseType(typeof(EnrollmentDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        {
            var result = await sender.Send(new GetEnrollmentByIdQuery(id), ct);

            if (result.IsFailure)
            {
                var statusCode = result.Error.Code switch
                {
                    "Enrollment.NotFound" => StatusCodes.Status404NotFound,
                    "Enrollment.Unauthorized" => StatusCodes.Status403Forbidden,
                    _ => StatusCodes.Status400BadRequest
                };

                return Problem(
                    detail: result.Error.Description,
                    title: result.Error.Code,
                    statusCode: statusCode);
            }

            return Ok(result.Value);
        }
        /// <summary>
        /// Get all students enrolled in a course with their progress.
        /// Instructors can only see enrollments for their own courses.
        /// Admins and SuperAdmins can see any course.
        /// </summary>
        [HttpGet("course/{courseId:guid}")]
        [Authorize(Roles = "Instructor,Admin,SuperAdmin")]
        [ProducesResponseType(typeof(CourseEnrollmentsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetByCourse(Guid courseId, CancellationToken ct)
        {
            var result = await sender.Send(
                new GetCourseEnrollmentsQuery(courseId), ct);

            if (result.IsFailure)
            {
                var statusCode = result.Error.Code switch
                {
                    "Course.NotFound" => StatusCodes.Status404NotFound,
                    "Course.Unauthorized" => StatusCodes.Status403Forbidden,
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
}
