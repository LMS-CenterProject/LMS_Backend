using LMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Domain.Interfaces.Repositories
{
    public interface ILessonRepository
    {
        Task<int> CountByCourseAsync(
        Guid courseId,
        CancellationToken ct = default);

        Task<Lesson?> GetByIdWithSectionAsync(
            Guid lessonId,
            CancellationToken ct = default);
    }
}
