using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Quiz.Command.SubmitQuiz
{
    using LMS.Application.DTOs.Quize;
    using MediatR;

    public record SelectedAnswer(
        Guid QuestionId,
        Guid AnswerId
    );

    public record SubmitQuizCommand(
        Guid QuizId,
        List<SelectedAnswer> Answers
    ) : IRequest<QuizResultDto>;
}
