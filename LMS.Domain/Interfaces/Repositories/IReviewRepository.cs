using LMS.Domain.Entities;
using LMS.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Domain.Interfaces.Repositories
{

    public interface IReviewRepository : IRepository<Review>
    {
        Task<Review?> GetByIdAsync(
            Guid reviewId,
            CancellationToken ct = default);

        Task<bool> ExistsAsync(
            Guid studentId,
            Guid targetId,
            ReviewTargetType targetType,
            CancellationToken ct = default);

        Task<IEnumerable<Review>> GetByTargetAsync(
            Guid targetId,
            ReviewTargetType targetType,
            CancellationToken ct = default);

        Task<double> GetAverageRatingAsync(
            Guid targetId,
            ReviewTargetType targetType,
            CancellationToken ct = default);
    }
}
