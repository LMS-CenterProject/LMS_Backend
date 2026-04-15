
using LMS.Domain.Interfaces;
using LMS.Domain.Interfaces.Repositories;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LMS.Application.Features.Lesson.Commands.ToggleFreePreview
{
    public sealed class ToggleFreePreviewHandler
        : IRequestHandler<ToggleFreePreviewCommand, bool>
    {
        private readonly ILessonRepository _lessonRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ToggleFreePreviewHandler(
            ILessonRepository lessonRepository,
            IUnitOfWork unitOfWork)
        {
            _lessonRepository = lessonRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(ToggleFreePreviewCommand request, CancellationToken cancellationToken)
        {
            var lesson = await _lessonRepository.GetByIdAsync(request.LessonId, cancellationToken);
            if (lesson == null)
                return false;

            lesson.ToggleFreePreview();     // Business method من الـ Domain

            _lessonRepository.Update(lesson);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}