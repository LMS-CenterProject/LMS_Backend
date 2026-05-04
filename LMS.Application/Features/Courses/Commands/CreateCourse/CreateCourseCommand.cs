using LMS.Application.Courses.DTOs;
using LMS.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Courses.Commands.CreateCourse
{
    public sealed record CreateCourseCommand(
       Guid? InstructorId,   // null  → use caller's own ID (Instructor role)
                             // value → assign to this instructor (Admin only)
       Guid CategoryId,
       string Title,
       string? Description,
       string? ThumbnailUrl,
       decimal Price,
       CourseLevel Level,
       string Language
   ) : IRequest<CreateCourseResponse>;



}
