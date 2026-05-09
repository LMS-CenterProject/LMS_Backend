using LMS.Application.DTOs.Answers;
using LMS.Application.Features.Answer.Command.CreateAnswer;
using LMS.Application.Features.Answer.Command.DeleteAnswer;
using LMS.Application.Features.Answer.Command.UpdateAnswer;
using LMS.Application.Features.Answer.Query.GetAnswerByQuestion;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AnswerController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AnswerController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET api/answers/question/{questionId}
        [HttpGet("question/{questionId}")]
        [Authorize(Policy = "ReadAnswer")]
        public async Task<IActionResult> GetByQuestion(Guid questionId)
        {
            var result = await _mediator.Send(
                new GetAnswerByQuestionIdQuery(questionId));
            if (User.IsInRole("Instructor"))
                return Ok(result); 

            
            return Ok(result.Select(a => new AnswerDto
            {
                Id = a.Id,
                Text = a.Text
            }));
        }

        // POST api/answers
        [HttpPost]
        [Authorize(Policy = "ManageAnswer")]
        public async Task<IActionResult> Create(
            CreateAnswerCommand command)
        {
            var id = await _mediator.Send(command);

            return Ok(id);
        }

        // PUT api/answers/{id}
        [HttpPut("{id}")]
        [Authorize(Policy = "ManageAnswer")]
        public async Task<IActionResult> Update(
            Guid id,
            UpdateAnswerCommand command)
        {
            var updated = command with { AnswerId = id };

            await _mediator.Send(updated);

            return NoContent();
        }

        // DELETE api/answers/{id}
        [HttpDelete("{id}")]
        [Authorize(Policy = "ManageAnswer")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _mediator.Send(
                new DeleteAnswerCommand(id));

            return NoContent();
        }
    }
}