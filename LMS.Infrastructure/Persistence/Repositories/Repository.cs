using LMS.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace LMS.Infrastructure.Persistence.Repositories
{
    public class Repository<T>(LMSDbContext context) : IRepository<T> where T : class
    {
        protected readonly LMSDbContext Context = context;
        protected readonly DbSet<T> DbSet = context.Set<T>();

        public async Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
            await DbSet.FindAsync([id], ct);

        public async Task<IEnumerable<T>> GetAllAsync(CancellationToken ct = default) =>
            await DbSet.ToListAsync(ct);

        public async Task<IEnumerable<T>> FindAsync(
            Expression<Func<T, bool>> predicate, CancellationToken ct = default) =>
            await DbSet.Where(predicate).ToListAsync(ct);

        public async Task AddAsync(T entity, CancellationToken ct = default) =>
            await DbSet.AddAsync(entity, ct);

        public void Update(T entity) => DbSet.Update(entity);

        public void Remove(T entity) => DbSet.Remove(entity);
    }
}
