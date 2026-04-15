using LMS.Application.DTOs.Course;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Courses.Queries.GetCourseDetails
{
    public sealed record GetCourseDetailsQuery(Guid CourseId) : IRequest<CourseDetailDto>;
}
