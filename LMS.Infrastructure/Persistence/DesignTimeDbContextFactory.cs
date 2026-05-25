using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace LMS.Infrastructure.Persistence
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<LMSDbContext>
    {
        public LMSDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<LMSDbContext>();

            optionsBuilder.UseSqlServer(
                "Server=db53550.public.databaseasp.net; Database=db53550; User Id=db53550; Password=Mf8-d2_K#pH6; Encrypt=True; TrustServerCertificate=True; MultipleActiveResultSets=True; ");

            return new LMSDbContext(optionsBuilder.Options);
        }
    }
}