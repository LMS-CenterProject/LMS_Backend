using LMS.Application.Common.Interfaces;
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
    public sealed class AdminController(
        ISender sender,
        ICurrentUserService currentUserService,
        ILogger<AdminController> logger) : ControllerBase
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
            logger.LogInformation(
                "Admin {AdminUser} ({AdminRole}) requested users page {Page} with page size {PageSize}, search {SearchTerm}, role filter {RoleFilter}",
                GetActorLabel(),
                GetActorRole(),
                page,
                pageSize,
                search ?? "<none>",
                role?.ToString() ?? "<none>");

            var result = await sender.Send(
                new GetAllUsersQuery(page, pageSize, search, role), ct);

            if (result.IsSuccess)
            {
                logger.LogInformation(
                    "Admin {AdminUser} retrieved users page {Page}: {ReturnedCount} users returned out of {TotalCount} total",
                    GetActorLabel(),
                    result.Value.Page,
                    result.Value.Users.Count(),
                    result.Value.TotalCount);

                return Ok(result.Value);
            }

            logger.LogWarning(
                "Admin {AdminUser} failed to retrieve users: {ErrorCode} - {ErrorDescription}",
                GetActorLabel(),
                result.Error.Code,
                result.Error.Description);

            return BadRequest(result.Error.Description);
        }

        /// <summary>
        /// Activate a user account.
        /// </summary>
        [HttpPatch("users/{id:guid}/activate")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Activate(Guid id, CancellationToken ct)
        {
            logger.LogInformation(
                "Admin {AdminUser} requested activation for user {TargetUserId}",
                GetActorLabel(),
                id);

            var result = await sender.Send(new ActivateUserCommand(id), ct);

            if (result.IsSuccess)
            {
                logger.LogInformation(
                    "Admin {AdminUser} activated user {TargetUserId}",
                    GetActorLabel(),
                    id);

                return NoContent();
            }

            logger.LogWarning(
                "Admin {AdminUser} failed to activate user {TargetUserId}: {ErrorCode} - {ErrorDescription}",
                GetActorLabel(),
                id,
                result.Error.Code,
                result.Error.Description);

            return Problem(result.Error.Description,
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
            logger.LogInformation(
                "Admin {AdminUser} requested deactivation for user {TargetUserId}",
                GetActorLabel(),
                id);

            var result = await sender.Send(new DeactivateUserCommand(id), ct);

            if (result.IsFailure)
            {
                var statusCode = result.Error.Code switch
                {
                    "User.NotFound" => StatusCodes.Status404NotFound,
                    "User.CannotDeactivateSelf" => StatusCodes.Status400BadRequest,
                    _ => StatusCodes.Status400BadRequest
                };

                logger.LogWarning(
                    "Admin {AdminUser} failed to deactivate user {TargetUserId}: {ErrorCode} - {ErrorDescription}",
                    GetActorLabel(),
                    id,
                    result.Error.Code,
                    result.Error.Description);

                return Problem(result.Error.Description,
                    title: result.Error.Code,
                    statusCode: statusCode);
            }

            logger.LogInformation(
                "Admin {AdminUser} deactivated user {TargetUserId}",
                GetActorLabel(),
                id);

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
            logger.LogInformation(
                "Admin {AdminUser} ({AdminRole}) requested role change for user {TargetUserId} to {NewRole}",
                GetActorLabel(),
                GetActorRole(),
                id,
                request.NewRole);

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

                logger.LogWarning(
                    "Admin {AdminUser} failed to change role for user {TargetUserId} to {NewRole}: {ErrorCode} - {ErrorDescription}",
                    GetActorLabel(),
                    id,
                    request.NewRole,
                    result.Error.Code,
                    result.Error.Description);

                return Problem(result.Error.Description,
                    title: result.Error.Code,
                    statusCode: statusCode);
            }

            logger.LogInformation(
                "Admin {AdminUser} changed role for user {TargetUserId} to {NewRole}",
                GetActorLabel(),
                id,
                request.NewRole);

            return NoContent();
        }

        private string GetActorLabel()
        {
            var userId = currentUserService.UserId?.ToString() ?? "unknown";
            var displayName = currentUserService.DisplayName;

            return string.IsNullOrWhiteSpace(displayName)
                ? userId
                : $"{displayName} ({userId})";
        }

        private string GetActorRole() =>
            currentUserService.Role ?? "unknown";
    }

    public sealed record ChangeRoleRequest(UserRole NewRole);
}
