using LMS.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Courses.Commands.UpdateCourse
{
    public sealed record UpdateCourseCommand(
        Guid CourseId,
        string Title,
        string? Description,
        decimal Price,
        CourseLevel Level,
        string Language,
        string? ThumbnailUrl,
        Guid CategoryId) : IRequest;
    
}
