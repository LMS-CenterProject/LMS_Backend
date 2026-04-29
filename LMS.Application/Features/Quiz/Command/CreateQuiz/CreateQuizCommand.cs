using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Quiz.Command.CreateQuiz
{
    public record CreateQuizCommand(
    Guid CourseId,
    string Title,
    int PassScore,
    int? TimeLimitMinutes
) : IRequest<Guid>;
}
