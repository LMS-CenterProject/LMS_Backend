using LMS.Application.Features.Quiz.Command.CreateQuiz;
using LMS.Application.Features.Quiz.Command.DeleteQuiz;
using LMS.Application.Features.Quiz.Command.UpdateQuiz;
using LMS.Application.Features.Quiz.Query.GetCourseQuiz;
using LMS.Application.Features.Quiz.Query.GetQuiz;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class QuizController : ControllerBase
    {
        private readonly IMediator _mediator;

        public QuizController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET api/quizzes/{id}
        [HttpGet("{id}")]
        [Authorize(Policy = "ReadQuiz")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(
                new GetQuizByIdQuery(id));

            return Ok(result);
        }
       

        // GET api/quizzes/course/{courseId}
        [HttpGet("course/{courseId}")]
        [Authorize(Policy = "ReadQuiz")]
        public async Task<IActionResult> GetByCourse(Guid courseId)
        {
            var result = await _mediator.Send(
                new GetCourseQuizzesQuery(courseId));

            return Ok(result);
        }
   

        // POST api/quizzes
        [HttpPost]
        [Authorize(Policy = "ManageQuiz")]
        public async Task<IActionResult> Create(
            CreateQuizCommand command)
        {
            var id = await _mediator.Send(command);

            return Ok(id);
        }

        // PUT api/quizzes/{id}
        [HttpPut("{id}")]
        [Authorize(Policy = "ManageQuiz")]
        public async Task<IActionResult> Update(
            Guid id,
            UpdateQuizCommand command)
        {
            var updated = command with { QuizId = id };

            await _mediator.Send(updated);

            return NoContent();
        }

        // DELETE api/quizzes/{id}
        [HttpDelete("{id}")]
        [Authorize(Policy = "ManageQuiz")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _mediator.Send(
                new DeleteQuizCommand(id));

            return NoContent();
        }
    }
}