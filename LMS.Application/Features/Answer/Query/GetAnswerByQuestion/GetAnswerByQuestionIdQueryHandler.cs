using LMS.Application.DTOs.Answers;
using LMS.Domain.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Answer.Query.GetAnswerByQuestion
{
    public sealed class GetAnswerByQuestionIdQueryHandler
        : IRequestHandler<GetAnswerByQuestionIdQuery, List<AnswerDto>>
    {
        private readonly IAnswerRepository _answerRepository;

        public GetAnswerByQuestionIdQueryHandler(
            IAnswerRepository answerRepository)
        {
            _answerRepository = answerRepository;
        }

        public async Task<List<AnswerDto>> Handle(
            GetAnswerByQuestionIdQuery request,
            CancellationToken cancellationToken)
        {
            var answers = await _answerRepository
                .GetByQuestionIdAsync(request.QuestionId, cancellationToken);

            return answers.Select(a => new AnswerDto
            {
                Id = a.Id,
                QuestionId = a.QuestionId,
                Text = a.Text,
                IsCorrect = a.IsCorrect
            }).ToList();
        }
    }
}
