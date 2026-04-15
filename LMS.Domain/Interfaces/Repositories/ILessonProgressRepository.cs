using LMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Domain.Interfaces.Repositories
{
    public interface ILessonProgressRepository:IRepository<LessonProgress>
    {
        Task<LessonProgress?> GetByEnrollmentAndLessonAsync(
        Guid enrollmentId,
        Guid lessonId,
        CancellationToken ct = default);

        Task<int> CountCompletedAsync(
            Guid enrollmentId,
            CancellationToken ct = default);
    }
}
