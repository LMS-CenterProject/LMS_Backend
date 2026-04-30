using LMS.Domain.Entities;
using LMS.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Infrastructure.Persistence.Repositories
{
    public class QuestionRepository
    : Repository<Question>, IQuestionRepository
    {
        private readonly LMSDbContext _context;

        public QuestionRepository(LMSDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Question>> GetByQuizIdAsync(
            Guid quizId,
            CancellationToken ct = default)
        {
            return await _context.Questions
                .Where(q => q.QuizId == quizId)
                .Include(q => q.Answers)
                .ToListAsync(ct);
        }
        public async Task<Question?> GetWithAnswersAsync(
            Guid questionId,
            CancellationToken ct = default)
        {
            return await Context.Questions
                .Include(q => q.Answers)
                .FirstOrDefaultAsync(q => q.Id == questionId, ct);
        }
    }
}
