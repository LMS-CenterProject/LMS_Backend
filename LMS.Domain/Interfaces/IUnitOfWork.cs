using LMS.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;

namespace LMS.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
            //The repositories are read-only({ get; }with no setter) by design in the Unit of Work pattern.
            //It prevents anyone from accidentally replacing a repository(which would break the shared DbContext and the single transaction).
            //This is the standard, recommended way in Clean Architecture / DDD.
        IUserRepository Users { get; }
        IRefreshTokenRepository RefreshTokens { get; }
        IEnrollmentRepository Enrollments { get; }
        ICourseRepository Courses { get; }
        ILessonProgressRepository LessonProgresses { get; }
        ILessonRepository Lessons { get; }
        ICertificateRepository Certificates { get; }

        Task<int> SaveChangesAsync(CancellationToken ct = default);
    }
}
