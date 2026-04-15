using LMS.Application.Common.Interfaces;
using LMS.Domain.Interfaces;
using LMS.Domain.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Courses.Commands.DeleteCourse
{
    public sealed class DeleteCourseHandler
        : IRequestHandler<DeleteCourseCommand>
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public DeleteCourseHandler(
            ICourseRepository courseRepository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService)
        {
            _courseRepository = courseRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task Handle(
            DeleteCourseCommand request,
            CancellationToken cancellationToken)
        {
            var course = await _courseRepository
                .GetByIdAsync(request.CourseId, cancellationToken);

            if (course is null)
                throw new Exception("Course not found");

            var userId = _currentUserService.UserId;

            if (userId is null)
                throw new UnauthorizedAccessException();

            if (!course.IsOwner(userId.Value))
                throw new UnauthorizedAccessException();

            course.SoftDelete();
            _courseRepository.Update(course);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
