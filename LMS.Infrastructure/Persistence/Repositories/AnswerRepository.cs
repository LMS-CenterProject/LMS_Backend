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
    public sealed class AnswerRepository(LMSDbContext context)
        : Repository<Answer>(context), IAnswerRepository
    {
        public async Task<IEnumerable<Answer>> GetByQuestionIdAsync(
            Guid questionId,
            CancellationToken cancellationToken = default)
        {
            return await DbSet
                .Where(a => a.QuestionId == questionId)
                .ToListAsync(cancellationToken);
        }
    }
}
