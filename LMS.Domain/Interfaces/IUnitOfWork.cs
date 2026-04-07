using LMS.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        IRefreshTokenRepository RefreshTokens { get; }

        Task<int> SaveChangesAsync(CancellationToken ct = default);
    }
}
