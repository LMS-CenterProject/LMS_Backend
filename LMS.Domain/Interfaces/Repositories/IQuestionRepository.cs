using LMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Domain.Interfaces.Repositories
{
    public interface IQuestionRepository : IRepository<Question>
    {
        Task<IEnumerable<Question>> GetByQuizIdAsync(
            Guid quizId,
            CancellationToken ct = default);
        Task<Question?> GetWithAnswersAsync(Guid questionId, CancellationToken ct = default);
    }
}
