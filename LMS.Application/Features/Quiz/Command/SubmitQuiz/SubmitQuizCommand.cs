using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Quiz.Command.SubmitQuiz
{
    using LMS.Application.DTOs.Quize;
    using MediatR;

    public record SubmitQuizCommand(
        Guid QuizId,
        Dictionary<Guid, List<Guid>> Answers
    ) : IRequest<QuizResultDto>;
}
