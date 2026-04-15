using LMS.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Lesson.Command.AddLesson
{
    public sealed record AddLessonCommand(
        Guid SectionId,
        string Title,
        string ContentUrl,
        ContentType ContentType,
        int DurationSeconds,
        int OrderIndex,
        bool IsFreePreview = false
    ) : IRequest<Guid>;
}
