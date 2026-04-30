using LMS.Application.DTOs;
using LMS.Application.Features.Admin.Command.ActivateUser;
using LMS.Application.Features.Admin.Command.ChangeUserRole;
using LMS.Application.Features.Admin.Command.DeactivateUser;
using LMS.Application.Features.Users.Queries.GetAllUsers;
using LMS.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers
{
    [ApiController]
    [Route("api/admin")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public sealed class AdminController(ISender sender) : ControllerBase
    {
        /// <summary>
        /// Get all users with pagination, optional search and role filter.
        /// </summary>
        [HttpGet("users")]
        [ProducesResponseType(typeof(PagedUsersDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllUsers(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] UserRole? role = null,
            CancellationToken ct = default)
        {
            var result = await sender.Send(
                new GetAllUsersQuery(page, pageSize, search, role), ct);

            return result.IsSuccess
                ? Ok(result.Value)
                : BadRequest(result.Error.Description);
        }

        /// <summary>
        /// Activate a user account.
        /// </summary>
        [HttpPatch("users/{id:guid}/activate")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Activate(Guid id, CancellationToken ct)
        {
            var result = await sender.Send(new ActivateUserCommand(id), ct);

            return result.IsSuccess
                ? NoContent()
                : Problem(result.Error.Description,
                    title: result.Error.Code,
                    statusCode: StatusCodes.Status404NotFound);
        }

        /// <summary>
        /// Deactivate a user account. Cannot deactivate your own account.
        /// </summary>
        [HttpPatch("users/{id:guid}/deactivate")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Deactivate(Guid id, CancellationToken ct)
        {
            var result = await sender.Send(new DeactivateUserCommand(id), ct);

            if (result.IsFailure)
            {
                var statusCode = result.Error.Code switch
                {
                    "User.NotFound" => StatusCodes.Status404NotFound,
                    "User.CannotDeactivateSelf" => StatusCodes.Status400BadRequest,
                    _ => StatusCodes.Status400BadRequest
                };

                return Problem(result.Error.Description,
                    title: result.Error.Code,
                    statusCode: statusCode);
            }

            return NoContent();
        }

        /// <summary>
        /// Change a user's role. Cannot change your own role.
        /// Only SuperAdmin can assign the SuperAdmin role.
        /// </summary>
        [HttpPatch("users/{id:guid}/role")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> ChangeRole(
            Guid id,
            [FromBody] ChangeRoleRequest request,
            CancellationToken ct)
        {
            var result = await sender.Send(
                new ChangeUserRoleCommand(id, request.NewRole), ct);

            if (result.IsFailure)
            {
                var statusCode = result.Error.Code switch
                {
                    "User.NotFound" => StatusCodes.Status404NotFound,
                    "User.CannotChangeOwnRole" => StatusCodes.Status400BadRequest,
                    "User.Unauthorized" => StatusCodes.Status403Forbidden,
                    _ => StatusCodes.Status400BadRequest
                };

                return Problem(result.Error.Description,
                    title: result.Error.Code,
                    statusCode: statusCode);
            }

            return NoContent();
        }
    }

    public sealed record ChangeRoleRequest(UserRole NewRole);
}

