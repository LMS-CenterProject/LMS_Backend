using LMS.Domain.Interfaces;
using LMS.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Infrastructure.Persistence.Repositories
{
    public sealed class UnitOfWork(LMSDbContext context) : IUnitOfWork
    {
        private IUserRepository? _users;
        private IRefreshTokenRepository? _refreshTokens;
        private IEnrollmentRepository? _enrollments;
        private ICourseRepository? _courses;

        public IUserRepository Users => _users ??= new UserRepository(context);
        public IRefreshTokenRepository RefreshTokens => _refreshTokens ??= new RefreshTokenRepository(context);
        public IEnrollmentRepository Enrollments => _enrollments ??= new EnrollmentRepository(context);
        public ICourseRepository Courses => _courses ??= new CourseRepository(context);

        public async Task<int> SaveChangesAsync(CancellationToken ct = default) =>
            await context.SaveChangesAsync(ct);

        public void Dispose() => context.Dispose();
    }
}
