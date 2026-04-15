using LMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Domain.Interfaces.Repositories
{
    public interface ILessonRepository:IRepository<Lesson>
    {
        Task<Lesson?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<Lesson>> GetLessonsBySectionIdAsync(Guid sectionId, CancellationToken cancellationToken = default);
        Task<List<Lesson>> GetActiveLessonsBySectionIdAsync(Guid sectionId, CancellationToken cancellationToken = default);
        Task<int> CountByCourseAsync(
        Guid courseId,
        CancellationToken ct = default);

        Task<Lesson?> GetByIdWithSectionAsync(
            Guid lessonId,
            CancellationToken ct = default);


    }
}
