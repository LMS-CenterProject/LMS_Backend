using LMS.Application.Common.Interfaces;
using LMS.Application.DTOs.Quize;
using LMS.Domain.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Quiz.Query.GetMyAttemp
{
    public class GetMyAttemptsQueryHandler
    : IRequestHandler<GetMyAttemptsQuery, List<QuizAttemptDto>>
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

        public async Task<List<QuizAttemptDto>> Handle(
            GetMyAttemptsQuery request,
            CancellationToken ct)
        {
            if (!_currentUser.UserId.HasValue)
                throw new UnauthorizedAccessException("User is not authenticated.");

            var studentId = _currentUser.UserId.Value;
            var attempts = await _repo
                .GetByStudentIdAsync(studentId, ct);

            return attempts
                .Select(a => new QuizAttemptDto
                {
                    Id = a.Id,           
                    QuizId = a.QuizId,
                    Score = a.Score,
                    Passed = a.Passed,
                    AttemptedAt = a.AttemptedAt
                })
                .ToList();
        }
    }
}
