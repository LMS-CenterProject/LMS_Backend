using LMS.Application.DTOs.Question;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Question.Query.GetQuestionByQuizId
{
    public record GetQuestionByQuizIdQuery(Guid QuizId)
    : IRequest<List<QuestionWithAnswersDto>>;
}
