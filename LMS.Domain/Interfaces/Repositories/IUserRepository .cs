using LMS.Domain.Entities;
using LMS.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Domain.Interfaces.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);
        Task<User?> GetByGoogleIdAsync(string googleId, CancellationToken ct = default);
        Task<bool> EmailExistsAsync(string email, CancellationToken ct = default);
        Task<(IEnumerable<User> Users, int TotalCount)> GetAllPaginatedAsync(
                    int page,
                    int pageSize,
                    string? search = null,
                    UserRole? role = null,
                    CancellationToken ct = default);
    }
}
