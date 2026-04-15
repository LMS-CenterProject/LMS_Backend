using LMS.Application.Common.Interfaces;
using LMS.Domain.Interfaces;
using LMS.Domain.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
namespace LMS.Application.Features.Courses.Commands.UpdateCourse
{
    public sealed class UpdateCourseCommandHandler : IRequestHandler<UpdateCourseCommand>
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public UpdateCourseCommandHandler(ICourseRepository courseRepository, IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _courseRepository = courseRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task Handle(
            UpdateCourseCommand request,
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
                throw new UnauthorizedAccessException("You are not the owner");

            course.Update(
                request.Title,
                request.Description,
                request.Price,
                request.Level,
                request.Language,
                request.ThumbnailUrl,
                request.CategoryId);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
