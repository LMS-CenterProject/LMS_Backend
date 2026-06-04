using LMS.Application.Common.Interfaces;
using LMS.Application.DTOs.Answers;
using LMS.Application.DTOs.Question;
using LMS.Application.DTOs.Quiz;
using LMS.Domain.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Quiz.Query.GetMyAttemp
{
    public class GetMyAttemptsQueryHandler
    : IRequestHandler<GetMyAttemptsQuery, List<StudentAttempDto>>
    {
        private readonly IQuizAttemptRepository _repo;
        private readonly ICurrentUserService _currentUser;

        public GetMyAttemptsQueryHandler(
            IQuizAttemptRepository repo,
            ICurrentUserService currentUser)
        {
            _repo = repo;
            _currentUser = currentUser;
        }

        public async Task<List<StudentAttempDto>> Handle(
            GetMyAttemptsQuery request,
            CancellationToken ct)
        {
            if (!_currentUser.UserId.HasValue)
                throw new UnauthorizedAccessException("User is not authenticated.");

            var studentId = _currentUser.UserId.Value;
            var attempts = await _repo
                .GetByStudentIdAsync(studentId, ct);

            return attempts.Select(a => new StudentAttempDto
            {
                AttemptId = a.Id,
                QuizId = a.QuizId,
                QuizTitle = a.Quiz.Title,       // needs .Include(a => a.Quiz)
                Score = a.Score,
                Passed = a.Passed,
                AttemptedAt = a.AttemptedAt,

                // Each QuizAttemptAnswer row = one question + one chosen answer
                Questions = a.Answers.Select(aa => new StudentQuestionWithAnswerDto
                {
                    QuestionId = aa.QuestionId,
                    QuestionText = aa.Question.Text,    // needs ThenInclude → Question
                    AnswerId = aa.AnswerId,
                    AnswerText = aa.Answer.Text,      // needs ThenInclude → Answer
                    IsCorrect = aa.Answer.IsCorrect  // true = student chose correctly
                }).ToList()

            }).ToList();
        }
    }
}

