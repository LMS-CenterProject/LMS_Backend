using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Answer.Command.CreateAnswer
{
    public sealed record CreateAnswerCommand(
        Guid QuestionId,
        string Text,
        bool IsCorrect
    ) : IRequest<Guid>;
}
