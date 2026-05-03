using LMS.Application.Features.Category.Command.CreateCategory;
using LMS.Application.Features.Category.Command.DeleteCategory;
using LMS.Application.Features.Category.Command.UpdateCategory;
using LMS.Application.Features.Category.Query.GetAll;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CategoryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetAllCategoriesQuery());
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Policy = "ManageCategory")]
        public async Task<IActionResult> Create(CreateCategoryCommand command)
        {
            var id = await _mediator.Send(command);
            return Ok(id);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "ManageCategory")]
        public async Task<IActionResult> Update(Guid id, UpdateCategoryCommand command)
        {
            var updated = command with { Id = id };
            await _mediator.Send(updated);
            return NoContent();
        }
        [HttpDelete("{id}")]
        [Authorize(Policy = "ManageCategory")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _mediator.Send(new DeleteCategoryCommand(id));
            return Ok("The Category has been deleted successfully.");
        }
    }
}
