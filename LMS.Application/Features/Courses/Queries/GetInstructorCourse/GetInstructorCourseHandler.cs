using LMS.Application.Common.Interfaces;
using LMS.Application.DTOs.Course;
using LMS.Domain.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Courses.Queries.GetInstructorCourse
{
    public sealed class GetInstructorCoursesQueryHandler
        : IRequestHandler<GetInstructorCoursesQuery, List<CourseInstractorDto>>
    {
        private readonly ICourseRepository _courseRepository;
        private readonly ICurrentUserService _currentUserService;

        public GetInstructorCoursesQueryHandler(ICourseRepository courseRepository, ICurrentUserService currentUserService)
        {
            _courseRepository = courseRepository;
            _currentUserService = currentUserService;
        }

        public async Task<List<CourseInstractorDto>> Handle(
            GetInstructorCoursesQuery request,
            CancellationToken cancellationToken)
        {
            var instructorId = _currentUserService.UserId;

            if (instructorId is null)
                throw new UnauthorizedAccessException("User not authenticated");
            var role = _currentUserService.Role;

            if (role != "Instructor" && role != "Admin")
                throw new UnauthorizedAccessException("Only instructors can access this");

            var courses = await _courseRepository.GetByInstructorIdAsync(instructorId.Value, cancellationToken);

            return courses.Select(course => new CourseInstractorDto
            {
                Id = course.Id,
                Title = course.Title,
                Description = course.Description,
                CategoryId = course.CategoryId,
                CategoryName = course.Category.Name,
                Thumbnail = course.ThumbnailUrl,
                Price = course.Price,
                Language = course.Language,
                Status = course.Status.ToString()
            }).ToList();
        }
    }
}
