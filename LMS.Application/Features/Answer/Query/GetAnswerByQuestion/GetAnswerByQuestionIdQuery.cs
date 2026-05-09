using LMS.Application.DTOs.Answers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Answer.Query.GetAnswerByQuestion
{
    public sealed record GetAnswerByQuestionIdQuery(
        Guid QuestionId
    ) : IRequest<List<AnswerQuestionDto>>;
}
