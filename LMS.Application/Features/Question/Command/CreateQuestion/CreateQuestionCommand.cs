using LMS.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Question.Command.CreateQuestion
{
    public record CreateQuestionCommand(
    Guid QuizId,
    string Text,
    QuestionType Type,
    int Points
) : IRequest<Guid>;
}
