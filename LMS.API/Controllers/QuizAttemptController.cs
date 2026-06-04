using LMS.Application.Features.Quiz.Command.SubmitQuiz;
using LMS.Application.DTOs.Quiz;
using LMS.Application.Features.Quiz.Query.GetMyAttemp;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using LMS.Application.Features.Attemps.Query.GetAttempForStudent;
using LMS.Application.Features.Attemps.Query.GetStudentAttempByName;

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
        [ProducesResponseType(typeof(IEnumerable<StudentAttempDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMyAttempts(CancellationToken ct)
        {
            var result = await _mediator.Send(new GetMyAttemptsQuery(), ct);
            return Ok(result);
        }
        [HttpGet("quiz/{quizId}/stats")]
        [Authorize(Policy = "ManageQuizAttempts")]  
        public async Task<IActionResult> GetQuizStats(Guid quizId)
        {
            var result = await _mediator.Send(new GetAttempForStudentQuery(quizId));
            return Ok(result);
        }

        [HttpGet("quiz/{quizId}/students")]
        [Authorize(Policy = "ManageQuizAttempts")]  // Instructor only
        public async Task<IActionResult> GetQuizStudentsStats(Guid quizId)
        {
            var result = await _mediator.Send(new GetStudentAttempByNameQuery(quizId));
            return Ok(result);
        }
    }
}
