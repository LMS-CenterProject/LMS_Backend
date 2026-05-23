using LMS.Application.DTOs.Course;
using LMS.Application.Features.Courses.Commands.ArchiveCourse;
using LMS.Application.Features.Courses.Commands.CreateCourse;
using LMS.Application.Features.Courses.Commands.DeleteCourse;
using LMS.Application.Features.Courses.Commands.PublishCourse;
using LMS.Application.Features.Courses.Commands.UpdateCourse;
using LMS.Application.Features.Courses.Queries.GetAllCourses;
using LMS.Application.Features.Courses.Queries.GetCourseById;
using LMS.Application.Features.Courses.Queries.GetCourseDetails;
using LMS.Application.Features.Courses.Queries.GetInstructorCourse;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CourseController : ControllerBase
    {
        private readonly IMediator _mediator;
        public CourseController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/Courses
        [HttpGet]
        [ProducesResponseType(typeof(CourseDto), 200)]
        [ProducesResponseType(404)]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllCourses()
        {
            var result = await _mediator.Send(new GetAllCoursesQuery());
            return Ok(result);
        }

        // GET: api/Courses/Instructor/{instructorId}
        [HttpGet("my-courses")]
        [Authorize(Policy = "GetInstructorCourses")]
        public async Task<IActionResult> GetInstructorCourses()
        {
            var result = await _mediator.Send(new GetInstructorCoursesQuery());
            return Ok(result);
        }

        // GET: api/Courses/{id}
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCourseById(Guid id)
        {
            var result = await _mediator.Send(new GetCourseByIdQuery(id));
            return result == null ? NotFound() : Ok(result);
        }

        // GET: api/Courses/{id}/details
        [HttpGet("{id}/details")]
        [ProducesResponseType(typeof(CourseDetailDto), 200)]
        [ProducesResponseType(404)]
        [AllowAnonymous]
        public async Task<IActionResult> GetCourseDetails(Guid id)
        {
            var result = await _mediator.Send(new GetCourseDetailsQuery(id));
            return result == null ? NotFound() : Ok(result);
        }

        // POST: api/Courses
        [HttpPost]
        [ProducesResponseType(typeof(CourseDto), 200)]
        [ProducesResponseType(404)]
        [Authorize(Policy = "ManageCourses")]
        public async Task<IActionResult> CreateCourse(CreateCourseCommand command)
        {
            var response = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetCourseById), new {id = response.CourseId},response );
        }

        // PUT: api/Courses/{id}
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(CourseDetailDto), 200)]
        [ProducesResponseType(404)]
        [Authorize(Policy = "ManageCourses")]
        public async Task<IActionResult> UpdateCourse(Guid id, [FromBody] UpdateCourseCommand command)
        {
            var updatedCommand = command with { CourseId = id };   
            await _mediator.Send(updatedCommand);
            return NoContent();
        }

        // PUT: api/Courses/{id}/publish
        [HttpPut("{id}/publish")]
        [Authorize(Policy = "ManageCourses")]
        public async Task<IActionResult> PublishCourse(Guid id)
        {
            await _mediator.Send(new PublishCourseCommand(id));
            return NoContent();
        }

        // PUT: api/Courses/{id}/archive
        [HttpPut("{id}/archive")]
        [Authorize(Policy = "ManageCourses")]
        public async Task<IActionResult> ArchiveCourse(Guid id)
        {
            await _mediator.Send(new ArchiveCourseCommand(id));
            return NoContent();
        }

        // DELETE: api/Courses/{id}
        [HttpDelete("{id}")]
        [Authorize(Policy = "ManageCourses")]
        public async Task<IActionResult> DeleteCourse(Guid id)
        {
            await _mediator.Send(new DeleteCourseCommand(id));
            return NoContent();
        }
    }
}
