using MediatR;

namespace LMS.Application.Features.Courses.Commands.ArchiveCourse
{
    public sealed record ArchiveCourseCommand(Guid CourseId) : IRequest;

}
