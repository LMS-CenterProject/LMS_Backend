using LMS.Application.DTOs.Course;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Courses.Queries.GetAllCourses
{
    public sealed record GetAllCoursesQuery() : IRequest<List<CourseDto>>;
}
