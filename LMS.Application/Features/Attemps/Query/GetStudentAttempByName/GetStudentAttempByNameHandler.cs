using LMS.Application.DTOs.Quiz;
using LMS.Domain.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Attemps.Query.GetStudentAttempByName
{
    public class GetStudentAttempByNameHandler : IRequestHandler<GetStudentAttempByNameQuery, QuizStudentsStatsDto>
    {
        private readonly IQuizAttemptRepository _attempRepository;
        private readonly IQuizRepository _quizRepo;
        public GetStudentAttempByNameHandler(IQuizAttemptRepository attempRepository, IQuizRepository quizRepo)
        {
            _attempRepository = attempRepository;
            _quizRepo = quizRepo;
        }
        public async Task<QuizStudentsStatsDto> Handle(GetStudentAttempByNameQuery request,CancellationToken ct)
        {
            var quiz = await _quizRepo.GetByIdAsync(request.QuizId, ct);
            if (quiz is null)
                throw new Exception($"Quiz '{request.QuizId}' not found.");

            var attempts = await _attempRepository.GetByQuizIdAsync(request.QuizId, ct);
            var studentStats = attempts.GroupBy(a => a.StudentId)
    .Select(g => new StudentAttemptSummaryDto
    {
        StudentId = g.Key,
        StudentName = g.First().Student.FullName,
        TotalAttempts = g.Count(),
        BestScore = g.Max(a => a.Score),
        LastAttempt = g.Max(a => a.AttemptedAt),
        Passed = g.Any(a => a.Passed)}).OrderByDescending(s => s.LastAttempt).ToList();

            return new QuizStudentsStatsDto
            {
                QuizId = request.QuizId,
                TotalStudents = studentStats.Count,
                TotalAttempts = attempts.Count(),
                Students = studentStats
            };
        }
    }
}
