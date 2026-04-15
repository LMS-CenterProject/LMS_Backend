using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Lesson.Command.DeleteLesson
{
    public sealed record DeleteLessonCommand(Guid LessonId) : IRequest<Unit>;
}
