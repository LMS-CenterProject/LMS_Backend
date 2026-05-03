using LMS.Application.Common.Interfaces;
using LMS.Application.Courses.DTOs;
using LMS.Application.Features.Courses.Commands.CreateCourse;
using LMS.Domain.Entities;
using LMS.Domain.Interfaces;
using LMS.Domain.Interfaces.Repositories;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace LMS.Application.Courses.Commands.CreateCourse
{
    public class CreateCourseCommandHandler
        : IRequestHandler<CreateCourseCommand, CreateCourseResponse>
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public CreateCourseCommandHandler(
            ICourseRepository courseRepository,
            IUnitOfWork unitOfWork,ICurrentUserService currentUserService)
        {
            _courseRepository = courseRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<CreateCourseResponse> Handle(CreateCourseCommand request, CancellationToken cancellationToken)
        {
            if (!_currentUserService.IsAuthenticated)
                throw new UnauthorizedAccessException("User is not authenticated");

            var instructorId = _currentUserService.UserId;

            if (instructorId is null)
                throw new Exception("Invalid user");


            var course = Course.Create(
                instructorId.Value,
                request.CategoryId,
                request.Title,
                request.Description,
                request.Price,
                request.Level,
                request.Language,
                request.ThumbnailUrl);

            await _courseRepository.AddAsync(course, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new CreateCourseResponse(
                course.Id,
                course.Title,
                course.Price
            );
        }
    }
}