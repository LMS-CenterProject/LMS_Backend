using LMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Domain.Interfaces.Repositories
{
    public interface IQuizAttemptRepository : IRepository<QuizAttempt>
    {
        Task<IEnumerable<QuizAttempt>> GetByStudentIdAsync(
            Guid studentId,
            CancellationToken ct = default);

        Task<IEnumerable<QuizAttempt>> GetByQuizIdAsync(
            Guid quizId,
            CancellationToken ct = default);
        Task<List<QuizAttempt>> GetByStudentAndQuizAsync(
            Guid studentId,
            Guid quizId,
            CancellationToken ct);

        Task<bool> HasStudentAttemptedAsync(
            Guid studentId,
            Guid quizId,
            CancellationToken ct = default);
        Task<int> CountAttemptsAsync(Guid studentId, Guid quizId, CancellationToken ct = default);
        Task<bool> HasPassedAsync(Guid studentId, Guid quizId, CancellationToken ct=default);

    }

}
