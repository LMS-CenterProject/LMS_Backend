using LMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Domain.Interfaces.Repositories
{
    public interface ICourseRepository: IRepository<Course>
    {
        Task<List<Course>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<Course?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<Course>> GetByInstructorIdAsync(Guid instructorId, CancellationToken cancellationToken = default);

        Task<Course?> GetDetailsByIdAsync(Guid id,CancellationToken cancellationToken = default);
        Task<Course?> GetByIdWithSectionsAsync(
            Guid courseId,
            CancellationToken ct = default);
    }
}
