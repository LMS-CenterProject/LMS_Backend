using LMS.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Question.Command.UpdateQuestion
{
    public record UpdateQuestionCommand(
    Guid QuestionId,
    string Text,
    QuestionType Type,
    int Points
) : IRequest;
}
