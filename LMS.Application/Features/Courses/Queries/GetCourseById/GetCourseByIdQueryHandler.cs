using FluentValidation;
using LMS.Application.DTOs.Course;
using LMS.Domain.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Courses.Queries.GetCourseById
{
    public  class GetCourseByIdQueryHandler: IRequestHandler<GetCourseByIdQuery, CourseDto>
    {
        private readonly ICourseRepository _courseRepository;
        public GetCourseByIdQueryHandler(ICourseRepository courseRepository)
        {
            _courseRepository = courseRepository;
        }

        public async Task<CourseDto> Handle(
            GetCourseByIdQuery request,
            CancellationToken cancellationToken)
        {
            var course = await _courseRepository
                .GetByIdAsync(request.CourseId, cancellationToken);

            if (course is null)
                throw new Exception("Course not found");

            return new CourseDto
            {
                Id = course.Id,
                Title = course.Title,
                Description = course.Description,
                Price = course.Price,
                Language = course.Language,
                Status = course.Status.ToString(),
            };
        }
    }
}
