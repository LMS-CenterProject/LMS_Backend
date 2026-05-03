using LMS.Domain.Entities;
using LMS.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Persistence.Repositories
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(LMSDbContext context) : base(context)
        {
        }
        public async Task<bool> ExistsByNameAsync(string name, CancellationToken ct)
        {
            return await DbSet.AnyAsync(c => c.Name == name, ct);
        }
        public async Task<Category?> GetByIdAsync(Guid id, CancellationToken ct)
        {
            return await DbSet
                .FirstOrDefaultAsync(c => c.Id == id, ct);
        }
    }
}
