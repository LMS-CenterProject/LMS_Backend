using LMS.Application.Features.Quiz.Command.SubmitQuiz;
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
        public async Task<IActionResult> Submit(SubmitQuizCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        // 🔥 My Attempts
        [HttpGet("my-attempts")]
        [Authorize(Policy = "ManageSubmit")]
        public async Task<IActionResult> GetMyAttempts()
        {
            var result = await _mediator.Send(new GetMyAttemptsQuery());
            return Ok(result);
        }
    }
}
