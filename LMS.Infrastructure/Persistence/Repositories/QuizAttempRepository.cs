using LMS.Domain.Entities;
using LMS.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Infrastructure.Persistence.Repositories
{
    public class QuizAttemptRepository
    : Repository<QuizAttempt>, IQuizAttemptRepository
    {
        private readonly LMSDbContext _context;

        public QuizAttemptRepository(LMSDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<QuizAttempt>> GetByStudentIdAsync(
            Guid studentId,
            CancellationToken ct = default)
        {
            return await _context.QuizAttempts
                .Where(a => a.StudentId == studentId).OrderByDescending(a => a.AttemptedAt)
                .ToListAsync(ct);
        }

        public async Task<IEnumerable<QuizAttempt>> GetByQuizIdAsync(
            Guid quizId,
            CancellationToken ct = default)
        {
            return await _context.QuizAttempts
                .Where(a => a.QuizId == quizId)
                .ToListAsync(ct);
        }

        public async Task<bool> HasStudentAttemptedAsync(
            Guid studentId,
            Guid quizId,
            CancellationToken ct = default)
        {
            return await _context.QuizAttempts
                .AnyAsync(a => a.StudentId == studentId && a.QuizId == quizId, ct);
        }
        public async Task<int> CountAttemptsAsync(Guid studentId, Guid quizId, CancellationToken ct = default)
        {
            return await Context.QuizAttempts
                .CountAsync(x => x.StudentId == studentId && x.QuizId == quizId, ct);
        }
        public async Task<bool> HasPassedAsync(Guid studentId, Guid quizId, CancellationToken ct = default)
        {
            return await Context.QuizAttempts
                .AnyAsync(x => x.StudentId == studentId
                            && x.QuizId == quizId
                            && x.Passed, ct);
        }
    }
}
