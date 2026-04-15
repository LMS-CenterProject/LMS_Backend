
using LMS.Application.DTOs.Course;
using MediatR;
namespace LMS.Application.Features.Courses.Queries.GetCourseById
{
    public sealed record GetCourseByIdQuery(Guid CourseId) : IRequest<CourseDto>;
}
