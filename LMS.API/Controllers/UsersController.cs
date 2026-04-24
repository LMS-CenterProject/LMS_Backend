using LMS.Application.DTOs;
using LMS.Application.Features.Users.Commands.ChangePassword;
using LMS.Application.Features.Users.Commands.UpdateProfile;
using LMS.Application.Features.Users.Queries.GetProfile;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers
{
    
    [ApiController]
    [Route("api/users")]
    [Authorize]
    public sealed class UsersController(ISender sender) : ControllerBase
    {
        /// <summary>
        /// Get the current user's profile.
        /// </summary>
        [HttpGet("me")]
        [ProducesResponseType(typeof(UserProfileDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProfile(CancellationToken ct)
        {
            var result = await sender.Send(new GetProfileQuery(), ct);

            return result.IsSuccess
                ? Ok(result.Value)
                : Problem(result.Error.Description,
                    title: result.Error.Code,
                    statusCode: StatusCodes.Status404NotFound);
        }

        /// <summary>
        /// Update the current user's profile (name, phone, avatar).
        /// </summary>
        [HttpPut("me")]
        [ProducesResponseType(typeof(UserProfileDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateProfile(
            [FromBody] UpdateProfileCommand command,
            CancellationToken ct)
        {
            var result = await sender.Send(command, ct);

            return result.IsSuccess
                ? Ok(result.Value)
                : Problem(result.Error.Description,
                    title: result.Error.Code,
                    statusCode: StatusCodes.Status400BadRequest);
        }

        /// <summary>
        /// Change the current user's password.
        /// Requires verifying the current password first.
        /// Not available for Google-only accounts.
        /// </summary>
        [HttpPut("me/password")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ChangePassword(
            [FromBody] ChangePasswordCommand command,
            CancellationToken ct)
        {
            var result = await sender.Send(command, ct);

            if (result.IsFailure)
            {
                var statusCode = result.Error.Code switch
                {
                    "User.WrongPassword" => StatusCodes.Status401Unauthorized,
                    "User.NoPasswordSet" => StatusCodes.Status400BadRequest,
                    _ => StatusCodes.Status400BadRequest
                };

                return Problem(result.Error.Description,
                    title: result.Error.Code,
                    statusCode: statusCode);
            }

            return NoContent();
        }
    }
}
