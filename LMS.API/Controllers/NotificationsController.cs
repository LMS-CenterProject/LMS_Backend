using LMS.Application.DTOs;
using LMS.Application.Features.Notifications.Commands;
using LMS.Application.Features.Notifications.Commands.MarkRead;
using LMS.Application.Features.Notifications.Queries.GetNotifications;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers
{
    /// <summary>
    /// Manage user notifications.
    /// </summary>
    [ApiController]
    [Route("api/notifications")]
    [Authorize]
    public sealed class NotificationsController(ISender  sender) : ControllerBase
    {
        /// <summary>
        /// Get the current user's notifications with pagination and unread count.
        /// </summary>
        /// <param name="page">Page number (default: 1)</param>
        /// <param name="pageSize">Items per page, max 50 (default: 20)</param>
        /// <param name="onlyUnread">Return only unread notifications (default: false)</param>
        [HttpGet]
        [ProducesResponseType(typeof(NotificationsResultDto), StatusCodes.Status200OK)]

        public async Task<IActionResult>GetAll(
            [FromQuery]int page =1,
            [FromQuery] int pageSize=20,
            [FromQuery]bool onlyUnread=false,
            CancellationToken ct =default)
        {
            var result = await sender.Send(
                new GetNotificationsQuery(page, pageSize, onlyUnread), ct);

            return result.IsSuccess
                ? Ok(result.Value)
                : BadRequest(result.Error.Description);
        }
        /// <summary>
        /// Mark a specific notification as read.
        /// Idempotent — calling multiple times is safe.
        /// </summary>
        [HttpPatch("{id:guid}/read")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> MarkRead(Guid id, CancellationToken ct)
        {
            var result = await sender.Send(new MarkReadCommand(id), ct);

            if (result.IsFailure)
            {
                var statusCode = result.Error.Code switch
                {
                    "Notification.NotFound" => StatusCodes.Status404NotFound,
                    "Notification.Unauthorized" => StatusCodes.Status403Forbidden,
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
        /// Mark all notifications as read for the current user.
        /// Returns the number of notifications that were marked.
        /// </summary>
        [HttpPatch("read-all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> MarkAllRead(CancellationToken ct)
        {
            var result = await sender.Send(new MarkAllReadCommand(), ct);

            return result.IsSuccess
                ? Ok(new { markedCount = result.Value })
                : BadRequest(result.Error.Description);
        }


    }
}
