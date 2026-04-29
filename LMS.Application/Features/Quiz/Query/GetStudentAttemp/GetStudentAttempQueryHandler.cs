using LMS.Application.Common.Interfaces;
using LMS.Application.DTOs.Quize;
using LMS.Domain.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Quiz.Query.GetStudentAttemp
{
    public class GetStudentAttemptQueryHandler
    : IRequestHandler<GetStudentAttemptQuery, List<QuizAttemptDto>>
    {
        private readonly IQuizAttemptRepository _attemptRepository;
        private readonly ICurrentUserService _currentUserService;
        public GetStudentAttemptQueryHandler(IQuizAttemptRepository attemptRepository, ICurrentUserService currentUserService)
        {
            _attemptRepository = attemptRepository;
            _currentUserService = currentUserService;
        }
        public async Task<List<QuizAttemptDto>> Handle(GetStudentAttemptQuery request, CancellationToken ct)
        {
            var userId = _currentUserService.UserId!.Value;

            var attempts = await _attemptRepository
                .GetByStudentIdAsync(userId, ct);

            return attempts.Select(a => new QuizAttemptDto
            {
                QuizId = a.QuizId,
                Score = a.Score,
                Passed = a.Passed,
                AttemptedAt = a.AttemptedAt
            }).ToList();
        }
    }
}
