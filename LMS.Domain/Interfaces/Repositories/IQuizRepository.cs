using LMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Domain.Interfaces.Repositories
{
    public interface IQuizRepository:IRepository<Quiz>
    {

        Task<Quiz?> GetQuizWithQuestionsAsync(Guid id, CancellationToken cancellationToken = default);

        Task<IEnumerable<Quiz>> GetByCourseIdAsync(
        Guid courseId,
        CancellationToken ct = default);
    }
}
