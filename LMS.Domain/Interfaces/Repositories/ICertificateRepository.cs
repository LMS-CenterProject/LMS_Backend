using LMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Domain.Interfaces.Repositories
{
    public interface ICertificateRepository:IRepository<Certificate>
    {
        Task<IEnumerable<Certificate>> GetByStudentAsync(
      Guid studentId,
      CancellationToken ct = default);

        Task<bool> ExistsForEnrollmentAsync(
            Guid enrollmentId,
            CancellationToken ct = default);
        
    }
}
