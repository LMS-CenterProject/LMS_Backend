using LMS.Application.Common.Models;
using LMS.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Progress.Commands.UpdateLessonProgress
{
    public sealed record UpdateLessonProgressCommand(
    Guid LessonId,
    int WatchedSeconds,
    bool IsCompleted) : IRequest<Result<UpdateProgressResultDto>>;
}
