using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Question.Command.DeleteQuestion
{
    public record DeleteQuestionCommand(Guid QuestionId) : IRequest;
}
