using LMS.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Lesson.Command.UpdateLesson
{
    public sealed record UpdateLessonCommand(
        Guid LessonId,
        string Title,
        string ContentUrl,
        ContentType ContentType,
        int DurationSeconds,
        int OrderIndex,
        bool IsFreePreview
    ) : IRequest<Guid>;
}
