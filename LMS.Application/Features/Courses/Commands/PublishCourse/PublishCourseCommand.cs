using MediatR;

namespace LMS.Application.Features.Courses.Commands.PublishCourse
{
    public sealed record PublishCourseCommand(Guid CourseId) : IRequest;
}
