using LMS.Application.DTOs.Answers;
using LMS.Application.DTOs.Question;
using LMS.Domain.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Question.Query.GetQuestionWithAnswer
{
    public class GetQuestionDetailQueryHandler
    : IRequestHandler<GetQuestionDetailQuery, QuestionDetailDto>
    {
        private readonly IQuestionRepository _questionRepository;

        public GetQuestionDetailQueryHandler(IQuestionRepository questionRepository)
        {
            _questionRepository = questionRepository;
        }

        public async Task<QuestionDetailDto> Handle(
            GetQuestionDetailQuery request,
            CancellationToken ct)
        {
            var question = await _questionRepository.GetWithAnswersAsync(request.QuestionId, ct);

            if (question is null)
                throw new Exception("Question not found");

            return new QuestionDetailDto
            {
                Id = question.Id,
                QuizId = question.QuizId,
                Text = question.Text,
                Type = question.Type.ToString(),
                Points = question.Points,

                Answers = question.Answers.Select(a => new AnswerDto
                {
                    Id = a.Id,
                    Text = a.Text,
                }).ToList()
            };
        }
    }
}
