using LMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Domain.Interfaces.Repositories
{
    public interface ICourseRepository
    {
        Task<List<Course>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<Course?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<Course>> GetByInstructorIdAsync(Guid instructorId, CancellationToken cancellationToken = default);

        Task<Course?> GetDetailsByIdAsync(Guid id,CancellationToken cancellationToken = default);

        Task AddAsync(Course course, CancellationToken cancellationToken = default);

        void Update(Course course);

        void Delete(Course course);

    }
}
