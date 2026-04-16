using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LMS.Domain.Entities;
using LMS.Domain.Interfaces.Repositories;

namespace LMS.Infrastructure.Persistence.Repositories
{
    public class AnswerRepository
    : Repository<Answer>, IAnswerRepository
    {
        private readonly LMSDbContext _context;

        public AnswerRepository(LMSDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Answer>> GetByQuestionIdAsync(
            Guid questionId,
            CancellationToken ct = default)
        {
            return await _context.Answers
                .Where(a => a.QuestionId == questionId)
                .ToListAsync(ct);
        }
    }
}
