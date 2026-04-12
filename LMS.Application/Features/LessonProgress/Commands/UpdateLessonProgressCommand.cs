using LMS.Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.LessonProgress.Commands
{
    public record UpdateLessonProgressCommand(
     Guid LessonId,
     int WatchedSeconds,
     bool MarkAsCompleted
                    ) : IRequest<Result>;
}
