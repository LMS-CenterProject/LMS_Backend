using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Infrastructure.Persistence.Seeders
{
    public static class SeederExtensions
    {
        /// <summary>
        /// Applies pending migrations and seeds the database.
        /// Call this from Program.cs in Development only.
        /// </summary>
        public static async Task MigrateAndSeedAsync(this IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<LMSDbContext>();
            var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger("DatabaseSeeder");

            logger.LogInformation("Applying migrations...");
            await context.Database.MigrateAsync();

            logger.LogInformation("Starting database seeding...");
            await DatabaseSeeder.SeedAsync(context, logger);
            logger.LogInformation("Database seeding completed.");
        }
    }
}
