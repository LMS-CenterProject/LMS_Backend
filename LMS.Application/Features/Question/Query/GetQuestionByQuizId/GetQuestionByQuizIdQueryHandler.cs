using LMS.Application.DTOs.Question;
using LMS.Domain.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Question.Query.GetQuestionByQuizId
{
    public class GetQuestionsByQuizIdQueryHandler
    : IRequestHandler<GetQuestionByQuizIdQuery, List<QuestionDto>>
    {
        private readonly IQuestionRepository _questionRepository;

        public GetQuestionsByQuizIdQueryHandler(IQuestionRepository questionRepository)
        {
            _questionRepository = questionRepository;
        }

        public async Task<List<QuestionDto>> Handle(
            GetQuestionByQuizIdQuery request,
            CancellationToken ct)
        {
            var questions = await _questionRepository.GetByQuizIdAsync(request.QuizId, ct);

            return questions.Select(q => new QuestionDto
            {
                Id = q.Id,
                QuizId = q.QuizId,
                Text = q.Text,
                Type = q.Type.ToString(),
                Points = q.Points
            }).ToList();
        }
    }
}
