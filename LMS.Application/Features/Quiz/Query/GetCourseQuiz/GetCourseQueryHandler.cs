using LMS.Application.DTOs.Question;
using LMS.Application.DTOs.Quiz;
using LMS.Domain.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Quiz.Query.GetCourseQuiz
{
    public class GetCourseQuizzesHandler
    : IRequestHandler<GetCourseQuizzesQuery, List<QuizWithQuestionDto>>
    {
        private readonly IQuizRepository _quizRepository;
        public GetCourseQuizzesHandler(IQuizRepository quizRepository)
        {
            _quizRepository = quizRepository;
        }
        public async Task<List<QuizWithQuestionDto>> Handle(GetCourseQuizzesQuery request, CancellationToken ct)
        {
            var quizzes = await _quizRepository
                .GetByCourseIdAsync(request.CourseId, ct);

            return quizzes.Select(q => new QuizWithQuestionDto
            {
                Id = q.Id,
                Title = q.Title,
                CourseId = q.CourseId,
                PassScore = q.PassScore,
                TimeLimitMinutes = q.TimeLimitMinutes,
                Questions = q.Questions.Select(ques => new QuestionDto
                {
                    Id = ques.Id,
                    QuizId = ques.QuizId,
                    Text = ques.Text,
                    Points = ques.Points,
                    Type = ques.Type.ToString(),
                }).ToList()
            }).ToList();
        }
    }
}
