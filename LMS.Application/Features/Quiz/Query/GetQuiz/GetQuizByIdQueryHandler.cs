using LMS.Application.DTOs.Answers;
using LMS.Application.DTOs.Question;
using LMS.Application.DTOs.Quize;
using LMS.Domain.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Quiz.Query.GetQuiz
{
    public class GetQuizByIdHandler : IRequestHandler<GetQuizByIdQuery, QuizDto>
    {
        private readonly IQuizRepository _quizRepository;

        public GetQuizByIdHandler(IQuizRepository quizRepository)
        {
            _quizRepository = quizRepository;
        }

        public async Task<QuizDto> Handle(GetQuizByIdQuery request, CancellationToken ct)
        {
            var quiz = await _quizRepository
                .GetQuizWithQuestionsAsync(request.QuizId, ct);

            if (quiz is null)
                throw new Exception("Quiz not found");

            return new QuizDto
            {
                Id = quiz.Id,
                Title = quiz.Title,
                CourseId = quiz.CourseId,
                TimeLimitMinutes = quiz.TimeLimitMinutes,
                Questions = quiz.Questions.Select(q => new QuestionDto
                {
                    Id = q.Id,
                    QuizId = q.QuizId,
                    Text = q.Text,
                    Type = q.Type.ToString(),
                    Points = q.Points,
                }).ToList()
            };
        }
    }
}
