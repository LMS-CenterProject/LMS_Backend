using MediatR;

namespace LMS.Application.Features.Courses.Commands.DeleteCourse
{
    public sealed record DeleteCourseCommand(Guid CourseId) : IRequest;

}
