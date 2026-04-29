using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Answer.Command.DeleteAnswer
{
    public sealed record DeleteAnswerCommand(
        Guid AnswerId
    ) : IRequest<Unit>;
}
