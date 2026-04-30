using LMS.Application.DTOs.Quize;
using LMS.Domain.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Quiz.Query.GetCourseQuiz
{
    public class GetCourseQuizzesHandler
    : IRequestHandler<GetCourseQuizzesQuery, List<QuizDto>>
    {
        private readonly IQuizRepository _quizRepository;
        public GetCourseQuizzesHandler(IQuizRepository quizRepository)
        {
            _quizRepository = quizRepository;
        }
        public async Task<List<QuizDto>> Handle(GetCourseQuizzesQuery request, CancellationToken ct)
        {
            var quizzes = await _quizRepository
                .GetByCourseIdAsync(request.CourseId, ct);

            return quizzes.Select(q => new QuizDto
            {
                Id = q.Id,
                Title = q.Title,
                CourseId = q.CourseId,
                PassScore = q.PassScore
            }).ToList();
        }
    }
}
