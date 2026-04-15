
using LMS.Application.Common.Interfaces;
using LMS.Domain.Entities;
using LMS.Domain.Interfaces;
using LMS.Domain.Interfaces.Repositories;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LMS.Application.Features.Lesson.Command.DeleteLesson
{
    public sealed class DeleteLessonHandler
        : IRequestHandler<DeleteLessonCommand, Unit>
    {
        private readonly ILessonRepository _lessonRepository;
        private readonly ISectionRepository _sectionRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteLessonHandler(
            ILessonRepository lessonRepository,
            ISectionRepository sectionRepository,
            ICourseRepository courseRepository,
            ICurrentUserService currentUserService,
            IUnitOfWork unitOfWork)
        {
            _lessonRepository = lessonRepository;
            _unitOfWork = unitOfWork;
            _sectionRepository = sectionRepository;
            _courseRepository = courseRepository;
            _currentUserService = currentUserService;
        }

        public async Task<Unit> Handle(DeleteLessonCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;

            if (userId is null)
                throw new UnauthorizedAccessException();

            var lesson = await _lessonRepository.GetByIdAsync(request.LessonId, cancellationToken);

            if (lesson is null)
                throw new Exception("Lesson not found");

            var section = await _sectionRepository.GetByIdAsync(lesson.SectionId, cancellationToken);
            var course = await _courseRepository.GetByIdAsync(section.CourseId, cancellationToken);

            // 🔥 Ownership check
            if (!course.IsOwner(userId.Value))
                throw new UnauthorizedAccessException("Not your course");

            // Soft Delete
            lesson.SoftDelete();

            _lessonRepository.Update(lesson);        
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}