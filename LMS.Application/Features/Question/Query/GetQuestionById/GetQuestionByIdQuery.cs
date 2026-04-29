using LMS.Application.DTOs.Question;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Question.Query.GetQuestionById
{
    public record GetQuestionByIdQuery(Guid QuestionId)
    : IRequest<QuestionDto>;
}
