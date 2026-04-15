using LMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Domain.Interfaces.Repositories
{
    public interface ISectionRepository : IRepository<Section>
    {
        Task<Section?> GetByIdAsync(Guid id,CancellationToken cancellationToken = default);
        Task<List<Section>> GetSectionsByCourseIdAsync(Guid courseId, CancellationToken cancellationToken = default);
        Task<List<Section>> GetSectionsWithLessonsByCourseIdAsync(Guid courseId, CancellationToken cancellationToken = default);

    }
}
