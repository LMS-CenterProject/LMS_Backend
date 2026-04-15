using LMS.Application.DTOs.Course;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Courses.Queries.GetInstructorCourse
{
    public sealed record GetInstructorCoursesQuery()
        : IRequest<List<CourseDto>>;

}
