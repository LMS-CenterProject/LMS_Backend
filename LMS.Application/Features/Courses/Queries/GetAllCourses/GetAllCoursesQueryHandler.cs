using LMS.Application.DTOs.Course;
using LMS.Domain.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Courses.Queries.GetAllCourses
{
    public sealed class GetAllCoursesQueryHandler: IRequestHandler<GetAllCoursesQuery, List<CourseDto>>
    {
        private readonly ICourseRepository _courseRepository;
        public GetAllCoursesQueryHandler(ICourseRepository courseRepository)
        {
            _courseRepository = courseRepository;
        }
        public async Task<List<CourseDto>> Handle(GetAllCoursesQuery request, CancellationToken cancellationToken)
        {
            var courses = await _courseRepository.GetAllAsync(cancellationToken);
            if (!courses.Any())
                return new List<CourseDto>();
            return courses.Select(c => new CourseDto
            {
                Id = c.Id,
                Title = c.Title,
                Description = c.Description,
                ThumbnailUrl = c.ThumbnailUrl,
                InstructorName = c.Instructor.FullName,
                CategoryName = c.Category.Name,
                CategoryId = c.Category.Id,
                Price = c.Price,    
                Language = c.Language,          
                Status = c.Status.ToString(),
                SectionCount = c.Sections.Count(),
                LessonCount = c.Sections.Sum(s => s.Lessons.Count())

            }).ToList();

        }
    }
}
