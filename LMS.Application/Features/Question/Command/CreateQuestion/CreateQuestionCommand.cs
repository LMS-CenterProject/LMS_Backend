using LMS.Application.DTOs.Answers;
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
    //List<AnswerDto> Answers
) : IRequest<Guid>;
}
