using LMS.Application.DTOs;
using LMS.Application.Features.Reviews.Commands.CreateReview;
using LMS.Application.Features.Reviews.Commands.UpdateReview.LMS.Application.Features.Reviews.Commands.UpdateReview;
using LMS.Application.Features.Reviews.Queries.GetReviews;
using LMS.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers
{
    
    [ApiController]
    [Route("api/reviews")]
    public sealed class ReviewsController(ISender sender) : ControllerBase
    {
        /// <summary>
        /// Submit a review for a Course (0), Teacher (1), or Lesson (2).
        /// Student must be enrolled for Course/Teacher, or have completed the lesson for Lesson.
        /// </summary>
        /// /// <param name="targetId">The ID of the target.</param>
        /// <param name="targetType">The type of the target (0=Course, 1=Teacher, 2=Lesson).</param>
        [HttpPost]
        [Authorize(Roles = "Student")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Create(
            [FromBody] CreateReviewCommand command,
            CancellationToken ct)
        {
            var result = await sender.Send(command, ct);

            if (result.IsFailure)
            {
                var statusCode = result.Error.Code switch
                {
                    "Review.AlreadyExists" => StatusCodes.Status409Conflict,
                    "Review.NotEligible" => StatusCodes.Status403Forbidden,
                    "Review.LessonNotCompleted" => StatusCodes.Status403Forbidden,
                    "Review.CannotReviewOwn" => StatusCodes.Status403Forbidden,
                    _ => StatusCodes.Status400BadRequest
                };

                return Problem(
                    detail: result.Error.Description,
                    title: result.Error.Code,
                    statusCode: statusCode);
            }

            return CreatedAtAction(
                nameof(GetByTarget),
                new { targetId = command.TargetId, targetType = command.TargetType },
                new { reviewId = result.Value });
        }

        /// <summary>
        /// Update your own review. Only allowed within 30 days of posting.
        /// </summary>
        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Student")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(
            Guid id,
            [FromBody] UpdateReviewRequest request,
            CancellationToken ct)
        {
            var result = await sender.Send(
                new UpdateReviewCommand(id, request.Rating, request.Comment), ct);

            if (result.IsFailure)
            {
                var statusCode = result.Error.Code switch
                {
                    "Review.NotFound" => StatusCodes.Status404NotFound,
                    "Review.Unauthorized" => StatusCodes.Status403Forbidden,
                    "Review.EditWindowClosed" => StatusCodes.Status400BadRequest,
                    _ => StatusCodes.Status400BadRequest
                };

                return Problem(
                    detail: result.Error.Description,
                    title: result.Error.Code,
                    statusCode: statusCode);
            }

            return Ok();
        }

        /// <summary>
        /// Get all reviews for a target with average rating.
        /// TargetType: 0 = Course, 1 = Teacher, 2 = Lesson
        /// No authentication required — public endpoint.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ReviewsResultDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByTarget(
            [FromQuery] Guid targetId,
            [FromQuery] ReviewTargetType targetType,
            CancellationToken ct)
        {
            var result = await sender.Send(
                new GetReviewsQuery(targetId, targetType), ct);

            return result.IsSuccess
                ? Ok(result.Value)
                : BadRequest(result.Error.Description);
        }
    }

    // Separate request body for Update to keep the route id clean
    public sealed record UpdateReviewRequest(byte Rating, string? Comment);
}

