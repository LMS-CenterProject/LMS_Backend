using LMS.Application.Courses.DTOs;
using LMS.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Courses.Commands.CreateCourse
{
    public sealed record CreateCourseCommand(
        Guid InstructorId,
        Guid CategoryId,
        string Title,
        string? Description,
        string? ThumbnailUrl,
        decimal Price,
        CourseLevel Level,
        string Language
    ) : IRequest<CreateCourseResponse>;



}
