using LMS.Application.Features.Quiz.Command.SubmitQuiz;
using LMS.Application.DTOs.Quize;
using LMS.Application.Features.Quiz.Query.GetMyAttemp;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class QuizAttemptController : ControllerBase
    {
        private readonly IMediator _mediator;

        public QuizAttemptController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // 🔥 Submit Quiz
        [HttpPost("submit")]
        [Authorize(Policy = "ManageSubmit")]
        [ProducesResponseType(typeof(QuizResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Submit(    
            SubmitQuizCommand command,
            CancellationToken ct)
        {
            var result = await _mediator.Send(command, ct);
            return Ok(result);
        }

        // 🔥 My Attempts
        [HttpGet("my-attempts")]
        [Authorize(Policy = "ManageSubmit")]
        [ProducesResponseType(typeof(IEnumerable<QuizAttemptDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMyAttempts(CancellationToken ct)
        {
            var result = await _mediator.Send(new GetMyAttemptsQuery(), ct);
            return Ok(result);
        }
    }
}
