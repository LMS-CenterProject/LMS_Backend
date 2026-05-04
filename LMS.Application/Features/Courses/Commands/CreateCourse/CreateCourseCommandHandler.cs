using LMS.Application.Common.Interfaces;
using LMS.Application.Courses.DTOs;
using LMS.Application.Features.Courses.Commands.CreateCourse;
using LMS.Domain.Entities;
using LMS.Domain.Enums;
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
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly ICategoryRepository _categoryRepository;

        public CreateCourseCommandHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            ICategoryRepository categoryRepository)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _categoryRepository = categoryRepository;
        }

        public async Task<CreateCourseResponse> Handle(
            CreateCourseCommand request, CancellationToken cancellationToken)
        {
            // ── Auth guard ────────────────────────────────────────
            if (!_currentUserService.IsAuthenticated || _currentUserService.UserId is null)
                throw new UnauthorizedAccessException("User is not authenticated.");

            var callerId = _currentUserService.UserId.Value;
            var callerRole = _currentUserService.Role;
            var isAdmin = callerRole is "Admin" or "SuperAdmin";

            Guid instructorId;

            if (isAdmin)
            {
                // ── Admin path: must provide InstructorId ─────────
                if (request.InstructorId is null)
                    throw new ArgumentException(
                        "Admins must provide an InstructorId to assign the course. " +
                        "Use GET /api/admin/instructors to search for instructors.");

                var targetUser = await _unitOfWork.Users
                    .GetByIdAsync(request.InstructorId.Value, cancellationToken);

                if (targetUser is null)
                    throw new KeyNotFoundException(
                        $"Instructor with ID '{request.InstructorId}' was not found.");

                // Guard: target must actually be an Instructor
                // Admins and SuperAdmins cannot own courses — only Instructors can
                if (targetUser.Role != UserRole.Instructor)
                    throw new InvalidOperationException(
                        $"'{targetUser.FullName}' has role '{targetUser.Role}'. " +
                        $"Only users with the Instructor role can own a course. " +
                        $"Use PATCH /api/admin/users/{targetUser.Id}/role to change their role first.");

                // Guard: target must be active
                if (!targetUser.IsActive)
                    throw new InvalidOperationException(
                        $"Instructor '{targetUser.FullName}' is deactivated and cannot own courses.");

                instructorId = request.InstructorId.Value;
            }
            else
            {
                // ── Instructor path: always use their own ID ──────
                // Any InstructorId in the body is silently ignored
                instructorId = callerId;
            }

            var category = await _categoryRepository.GetByIdAsync(
                request.CategoryId,
                cancellationToken);

            if (category is null)
                throw new KeyNotFoundException(
                    $"Category with ID '{request.CategoryId}' was not found. " +
                    "Use GET /api/Category to choose a valid category.");

            if (category.IsDeleted)
                throw new InvalidOperationException(
                    $"Category '{category.Name}' is deleted and cannot be assigned to new courses.");

            // ── Create and persist ────────────────────────────────
            var course = Course.Create(
                instructorId: instructorId,
                categoryId: request.CategoryId,
                title: request.Title,
                description: request.Description,
                price: request.Price,
                level: request.Level,
                language: request.Language,
                thumbnailUrl: request.ThumbnailUrl);

            await _unitOfWork.Courses.AddAsync(course, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new CreateCourseResponse(
                course.Id,
                course.Title,
                course.Price);
        }
    }
}
