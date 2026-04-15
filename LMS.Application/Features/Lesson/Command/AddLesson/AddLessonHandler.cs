using LMS.Application.Common.Interfaces;
using LMS.Domain.Entities;
using LMS.Domain.Interfaces;
using LMS.Domain.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Lesson.Command.AddLesson
{
    public sealed class AddLessonHandler
        : IRequestHandler<AddLessonCommand, Guid>
    {
        private readonly ISectionRepository _sectionRepository;
        private readonly ILessonRepository _lessonRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public AddLessonHandler(
            ISectionRepository sectionRepository,
            ILessonRepository lessonRepository,
            ICourseRepository courseRepository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService)
        {
            _sectionRepository = sectionRepository;
            _lessonRepository = lessonRepository;
            _courseRepository = courseRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Guid> Handle(AddLessonCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;

            var section = await _sectionRepository.GetByIdAsync(request.SectionId);

            if (section is null)
                throw new Exception("Section not found");

            var course = await _courseRepository.GetByIdAsync(section.CourseId);

            // 🔥 Owner check
            if (!course.IsOwner(userId.Value))
                throw new UnauthorizedAccessException();

            var lesson = LMS.Domain.Entities.Lesson.Create(
                request.SectionId,
                request.Title,
                request.ContentUrl,
                request.ContentType,
                request.DurationSeconds,
                request.OrderIndex,
                request.IsFreePreview);

            await _lessonRepository.AddAsync(lesson, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return lesson.Id;
        }
    }
}
