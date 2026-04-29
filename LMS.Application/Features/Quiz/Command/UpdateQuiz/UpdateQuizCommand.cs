using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Quiz.Command.UpdateQuiz
{
    public record UpdateQuizCommand(
    Guid QuizId,
    string Title,
    int PassScore,
    int? TimeLimitMinutes
) : IRequest;
}
