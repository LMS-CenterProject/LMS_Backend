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
        .Where(a => a.StudentId == studentId)
        .OrderByDescending(a => a.AttemptedAt)
        .Include(a => a.Quiz)                        // quiz title
        .Include(a => a.Answers)                     // QuizAttemptAnswer rows
            .ThenInclude(aa => aa.Question)          // question text
        .Include(a => a.Answers)
            .ThenInclude(aa => aa.Answer)            // answer text + IsCorrect
        .ToListAsync(ct);
        }

        public async Task<IEnumerable<QuizAttempt>> GetByQuizIdAsync(
            Guid quizId,
            CancellationToken ct = default)
        {
            return await _context.QuizAttempts
                .Where(a => a.QuizId == quizId)
                .Include(a => a.Student)
                .OrderByDescending(a => a.AttemptedAt)
                .ToListAsync(ct);
        }
        public async Task<List<QuizAttempt>> GetByStudentAndQuizAsync(
            Guid studentId,
            Guid quizId,
            CancellationToken ct)
        {
            return await _context.QuizAttempts
                .Where(a => a.StudentId == studentId && a.QuizId == quizId)
                .OrderBy(a => a.AttemptedAt)
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
        public async Task AddAsync(QuizAttempt attempt, CancellationToken ct)
        {
            // EF Core tracks the child QuizAttemptAnswer rows automatically
            // because they are added to attempt._answers before Add is called.
            await _context.QuizAttempts.AddAsync(attempt, ct);
        }
    }
}
