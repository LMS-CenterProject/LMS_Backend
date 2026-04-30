using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Answer.Command.UpdateAnswer
{
    public sealed record UpdateAnswerCommand(
        Guid AnswerId,
        string Text,
        bool IsCorrect
    ) : IRequest<Unit>;
}
