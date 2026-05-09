using LMS.Application.Features.Question.Command.CreateQuestion;
using LMS.Application.Features.Question.Command.DeleteQuestion;
using LMS.Application.Features.Question.Command.UpdateQuestion;
using LMS.Application.Features.Question.Query.GetQuestionById;
using LMS.Application.Features.Question.Query.GetQuestionByQuizId;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class QuestionController : ControllerBase
    {
        private readonly IMediator _mediator;

        public QuestionController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET api/questions/{id}
        [HttpGet("{id}")]
        [Authorize(Policy = "ReadQuiz")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(
                new GetQuestionByIdQuery(id));

            return Ok(result);
        }

        // GET api/questions/quiz/{quizId}
        [HttpGet("quiz/{quizId}")]
        [Authorize(Policy = "ReadQuiz")]
        public async Task<IActionResult> GetByQuiz(Guid quizId)
        {
            var result = await _mediator.Send(
                new GetQuestionByQuizIdQuery(quizId));

            return Ok(result);
        }
  
        // POST api/questions
        [HttpPost]
        [Authorize(Policy = "ManageQuestion")]
        public async Task<IActionResult> Create(
            CreateQuestionCommand command)
        {
            var id = await _mediator.Send(command);

            return Ok(id);
        }

        // PUT api/questions/{id}
        [HttpPut("{id}")]
        [Authorize(Policy = "ManageQuestion")]
        public async Task<IActionResult> Update(
            Guid id,
            UpdateQuestionCommand command)
        {
            var updated = command with { QuestionId = id };

            await _mediator.Send(updated);

            return NoContent();
        }

        // DELETE api/questions/{id}
        [HttpDelete("{id}")]
        [Authorize(Policy = "ManageQuestion")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _mediator.Send(
                new DeleteQuestionCommand(id));

            return NoContent();
        }
    }
}