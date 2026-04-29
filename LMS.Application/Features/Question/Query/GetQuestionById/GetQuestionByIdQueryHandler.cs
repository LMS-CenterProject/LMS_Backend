using LMS.Application.DTOs.Question;
using LMS.Domain.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Question.Query.GetQuestionById
{
    public class GetQuestionByIdQueryHandler
    : IRequestHandler<GetQuestionByIdQuery, QuestionDto>
    {
        private readonly IQuestionRepository _questionRepository;

        public GetQuestionByIdQueryHandler(IQuestionRepository questionRepository)
        {
            _questionRepository = questionRepository;
        }

        public async Task<QuestionDto> Handle(
            GetQuestionByIdQuery request,
            CancellationToken ct)
        {
            var question = await _questionRepository.GetByIdAsync(request.QuestionId, ct);

            if (question is null)
                throw new Exception("Question not found");

            return new QuestionDto
            {
                Id = question.Id,
                QuizId = question.QuizId,
                Text = question.Text,
                Type = question.Type.ToString(),
                Points = question.Points
            };
        }
    }
}
