using LMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Domain.Interfaces.Repositories
{
    public interface ICategoryRepository: IRepository<Category>
    {
        Task<bool> ExistsByNameAsync(string name, CancellationToken ct);
    }
}
