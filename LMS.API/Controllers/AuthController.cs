using LMS.Application.Common.Models;
using LMS.Application.Features.Auth.Commands.GoogleAuth;
using LMS.Application.Features.Auth.Commands.Login;
using LMS.Application.Features.Auth.Commands.Logout;
using LMS.Application.Features.Auth.Commands.RefreshToken;
using LMS.Application.Features.Auth.Commands.Register;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public sealed class AuthController(ISender sender) : ControllerBase
    {
        // POST api/auth/register
        [HttpPost("register")]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Register(
            [FromBody] RegisterCommand command,
            CancellationToken ct)
        {
            var result = await sender.Send(command, ct);

            if (result.IsFailure)
                return Problem(
                    detail: result.Error.Description,
                    title: result.Error.Code,
                    statusCode: StatusCodes.Status409Conflict);

            return CreatedAtAction(nameof(Register), result.Value);
        }

        // POST api/auth/login
        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login(
            [FromBody] LoginCommand command,
            CancellationToken ct)
        {
            var result = await sender.Send(command, ct);

            if (result.IsFailure)
                return Problem(
                    detail: result.Error.Description,
                    title: result.Error.Code,
                    statusCode: StatusCodes.Status401Unauthorized);

            return Ok(result.Value);
        }

        // POST api/auth/refresh
        [HttpPost("refresh")]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Refresh(
            [FromBody] RefreshTokenCommand command,
            CancellationToken ct)
        {
            var result = await sender.Send(command, ct);

            if (result.IsFailure)
                return Problem(
                    detail: result.Error.Description,
                    title: result.Error.Code,
                    statusCode: StatusCodes.Status401Unauthorized);

            return Ok(result.Value);
        }

        // POST api/auth/logout
        [HttpPost("logout")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Logout(
            [FromBody] LogoutCommand command,
            CancellationToken ct)
        {
            await sender.Send(command, ct);
            return NoContent();
        }

        // POST api/auth/google
        [HttpPost("google")]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Google(
            [FromBody] GoogleAuthCommand command,
            CancellationToken ct)
        {
            var result = await sender.Send(command, ct);

            if (result.IsFailure)
                return Problem(
                    detail: result.Error.Description,
                    title: result.Error.Code,
                    statusCode: StatusCodes.Status401Unauthorized);

            return Ok(result.Value);
        }
    }
}
