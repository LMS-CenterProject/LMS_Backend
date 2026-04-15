using LMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Domain.Interfaces.Repositories
{
    public interface ILessonRepository
    {
        Task<Lesson?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<Lesson>> GetLessonsBySectionIdAsync(Guid sectionId, CancellationToken cancellationToken = default);
        Task<List<Lesson>> GetActiveLessonsBySectionIdAsync(Guid sectionId, CancellationToken cancellationToken = default);

        // Commands
        Task AddAsync(Lesson lesson, CancellationToken cancellationToken = default);
        void Update(Lesson lesson);
        void Delete(Lesson lesson);
    }
}
