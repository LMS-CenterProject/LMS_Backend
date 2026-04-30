using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Quiz.Command.DeleteQuiz
{
    public record DeleteQuizCommand(Guid QuizId) : IRequest;
}
