using LMS.Domain.Entities;
using LMS.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Infrastructure.Persistence.Seeders
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAsync(LMSDbContext context, ILogger logger)
        {
            try
            {
                await SeedUsersAsync(context, logger);
                await SeedCategoriesAsync(context, logger);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while seeding the database.");
                throw;
            }
        }

        // ── Users ─────────────────────────────────────────────────

        private static async Task SeedUsersAsync(LMSDbContext context, ILogger logger)
        {
            if (await context.Users.AnyAsync())
            {
                logger.LogInformation("Users already seeded — skipping.");
                return;
            }

            var users = new List<User>
        {
            // ── Super Admin ───────────────────────────────────
            CreateUser(
                fullName:     "Super Admin",
                email:        "superadmin@lms.com",
                password:     "SuperAdmin@123",
                role:         UserRole.SuperAdmin,
                phoneNumber:  "+1000000000"),

            // ── Admin ─────────────────────────────────────────
            CreateUser(
                fullName:     "Admin User",
                email:        "admin@lms.com",
                password:     "Admin@123",
                role:         UserRole.Admin,
                phoneNumber:  "+1000000001"),

            // ── Instructors ───────────────────────────────────
            CreateUser(
                fullName:     "Ahmed Hassan",
                email:        "ahmed.hassan@lms.com",
                password:     "Instructor@123",
                role:         UserRole.Instructor,
                phoneNumber:  "+201001234567"),

            CreateUser(
                fullName:     "Sarah Johnson",
                email:        "sarah.johnson@lms.com",
                password:     "Instructor@123",
                role:         UserRole.Instructor,
                phoneNumber:  "+1234567890"),

            CreateUser(
                fullName:     "Mohamed Ali",
                email:        "mohamed.ali@lms.com",
                password:     "Instructor@123",
                role:         UserRole.Instructor,
                phoneNumber:  "+201112345678"),

            // ── Students ──────────────────────────────────────
            CreateUser(
                fullName:     "Omar Khaled",
                email:        "omar.khaled@lms.com",
                password:     "Student@123",
                role:         UserRole.Student,
                phoneNumber:  "+201234567890"),

            CreateUser(
                fullName:     "Nour Ibrahim",
                email:        "nour.ibrahim@lms.com",
                password:     "Student@123",
                role:         UserRole.Student,
                phoneNumber:  "+201345678901"),

            CreateUser(
                fullName:     "Layla Ahmed",
                email:        "layla.ahmed@lms.com",
                password:     "Student@123",
                role:         UserRole.Student,
                phoneNumber:  "+201456789012"),

            CreateUser(
                fullName:     "Youssef Mostafa",
                email:        "youssef.mostafa@lms.com",
                password:     "Student@123",
                role:         UserRole.Student,
                phoneNumber:  "+201567890123"),

            CreateUser(
                fullName:     "Hana Sami",
                email:        "hana.sami@lms.com",
                password:     "Student@123",
                role:         UserRole.Student,
                phoneNumber:  "+201678901234"),
        };

            await context.Users.AddRangeAsync(users);
            await context.SaveChangesAsync();

            logger.LogInformation(
                "Seeded {Count} users successfully.", users.Count);
        }

        // ── Categories ────────────────────────────────────────────

        private static async Task SeedCategoriesAsync(LMSDbContext context, ILogger logger)
        {
            if (await context.Categories.AnyAsync())
            {
                logger.LogInformation("Categories already seeded — skipping.");
                return;
            }

            var development = Category.Create("Development");
            var design = Category.Create("Design");
            var business = Category.Create("Business");
            var dataScience = Category.Create("Data Science");

            await context.Categories.AddRangeAsync(
                development, design, business, dataScience);

            await context.SaveChangesAsync();

            // Sub-categories (need parent IDs after SaveChanges)
            var subCategories = new List<Category>
        {
            Category.Create("Web Development",   development.Id),
            Category.Create("Mobile",            development.Id),
            Category.Create("DevOps",            development.Id),
            Category.Create("UI/UX",             design.Id),
            Category.Create("Graphic Design",    design.Id),
            Category.Create("Machine Learning",  dataScience.Id),
            Category.Create("Data Analysis",     dataScience.Id),
        };

            await context.Categories.AddRangeAsync(subCategories);
            await context.SaveChangesAsync();

            logger.LogInformation("Seeded categories successfully.");
        }

        // ── Helper ────────────────────────────────────────────────

        private static User CreateUser(
            string fullName,
            string email,
            string password,
            UserRole role,
            string? phoneNumber = null)
        {
            var user = User.CreateLocal(
                fullName: fullName,
                email: email,
                passwordHash: BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12),
                phoneNumber: phoneNumber);

            user.ChangeRole(role);
            return user;
        }
    }
}
