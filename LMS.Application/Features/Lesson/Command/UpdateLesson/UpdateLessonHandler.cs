using LMS.Application.Common.Interfaces;
using LMS.Application.Features.Lesson.Command.UpdateLesson;
using LMS.Domain.Interfaces;
using LMS.Domain.Interfaces.Repositories;
using MediatR;

namespace LMS.Application.Features.Lesson.Commands.UpdateLesson
{
    public sealed class UpdateLessonHandler
        : IRequestHandler<UpdateLessonCommand, Guid>
    {
        private readonly ILessonRepository _lessonRepository;
        private readonly ISectionRepository _sectionRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateLessonHandler(
            ILessonRepository lessonRepository,
            ISectionRepository sectionRepository,
            ICourseRepository courseRepository,
            ICurrentUserService currentUserService,
            IUnitOfWork unitOfWork)
        {
            _lessonRepository = lessonRepository;
            _sectionRepository = sectionRepository;
            _courseRepository = courseRepository;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(UpdateLessonCommand request, CancellationToken cancellationToken)
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

            // Update using business method from Domain
            lesson.Update(
                request.Title,
                request.ContentUrl,
                request.ContentType,
                request.DurationSeconds,
                request.OrderIndex,
                request.IsFreePreview);

            _lessonRepository.Update(lesson);           
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return lesson.Id;
        }
    }
}