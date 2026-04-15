using MediatR;

namespace LMS.Application.Features.Lesson.Commands.ToggleFreePreview
{
    public sealed record ToggleFreePreviewCommand(Guid LessonId) : IRequest<bool>;
}