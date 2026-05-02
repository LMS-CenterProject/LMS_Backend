using LMS.Application.DTOs.Course;
using LMS.Application.DTOs.Lesson;
using LMS.Domain.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Courses.Queries.GetCourseDetails
{
    public sealed class GetCourseDetailHandler : IRequestHandler<GetCourseDetailsQuery, CourseDetailDto>
    {
        private readonly ICourseRepository _courseRepository;
        public GetCourseDetailHandler(ICourseRepository courseRepository)
        {
            _courseRepository = courseRepository;
        }
        public async Task<CourseDetailDto> Handle(
            GetCourseDetailsQuery request,
            CancellationToken cancellationToken)
        {
            var course = await _courseRepository
                .GetDetailsByIdAsync(
                    request.CourseId,
                    cancellationToken);

            if (course is null)
                throw new Exception("Course not found");
            return new CourseDetailDto
            {
                Id = course.Id,
                Title = course.Title,
                Description = course.Description,
                Price = course.Price,
                Language = course.Language,
                Status = course.Status,
                Level = course.Level,
                InstructorId = course.InstructorId,
                CategoryId = course.CategoryId,
                InstructorName = course.Instructor.FullName,
                CategoryName = course.Category.Name,

                Sections = course.Sections
                    .OrderBy(s => s.OrderIndex)
                    .Select(section => new SectionDto
                    {
                        Id = section.Id,
                        Title = section.Title,
                        OrderIndex = section.OrderIndex,

                        Lessons = section.Lessons
                            .OrderBy(l => l.OrderIndex)
                            .Select(lesson => new LessonDto
                            {
                                Id = lesson.Id,
                                Title = lesson.Title,
                                DurationSeconds = lesson.DurationSeconds
                            })
                            .ToList()
                    })
                    .ToList()
            };


        }
    }
}
