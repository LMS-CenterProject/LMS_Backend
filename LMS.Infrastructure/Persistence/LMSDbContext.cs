using LMS.Domain.Entities;
using LMS.Domain.Enums;
using LMS.Domain.Primitives;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Infrastructure.Persistence
{
    public sealed class LMSDbContext(DbContextOptions<LMSDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users => Set<User>();
        
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
        public DbSet<Course> Courses => Set<Course>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Tag> Tags => Set<Tag>();
        public DbSet<CourseTag> CourseTags => Set<CourseTag>();
        public DbSet<Section> Sections => Set<Section>();
        public DbSet<Lesson> Lessons => Set<Lesson>();
        public DbSet<Enrollment> Enrollments => Set<Enrollment>();
        public DbSet<LessonProgress> LessonProgresses => Set<LessonProgress>();
        public DbSet<Certificate> Certificates => Set<Certificate>();
        public DbSet<Quiz> Quizzes => Set<Quiz>();
        public DbSet<Question> Questions => Set<Question>();
        public DbSet<Answer> Answers => Set<Answer>();
        public DbSet<QuizAttempt> QuizAttempts => Set<QuizAttempt>();
        public DbSet<Review> Reviews => Set<Review>();
        public DbSet<Notification> Notifications => Set<Notification>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            // CourseTag composite key
            modelBuilder.Entity<CourseTag>()
                .HasKey(ct => new { ct.CourseId, ct.TagId });

            // ── Fix cascade cycles ────────────────────────────────────
            modelBuilder.Entity<Course>()
                .HasOne(c => c.Instructor)
                .WithMany(u => u.Courses)
                .HasForeignKey(c => c.InstructorId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Course>()
                .HasOne(c => c.Category)
                .WithMany(cat => cat.Courses)
                .HasForeignKey(c => c.CategoryId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Student)
                .WithMany(u => u.Enrollments)
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Course)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<QuizAttempt>()
                .HasOne(a => a.Student)
                .WithMany()
                .HasForeignKey(a => a.StudentId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<QuizAttempt>()
                .HasOne(a => a.Quiz)
                .WithMany(q => q.Attempts)
                .HasForeignKey(a => a.QuizId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Student)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.StudentId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<LessonProgress>()
                .HasOne(lp => lp.Lesson)
                .WithMany(l => l.Progresses)
                .HasForeignKey(lp => lp.LessonId)
                .OnDelete(DeleteBehavior.NoAction);

            // ── Unique indexes ────────────────────────────────────────
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email).IsUnique();

            modelBuilder.Entity<Enrollment>()
                .HasIndex(e => new { e.StudentId, e.CourseId }).IsUnique();

            modelBuilder.Entity<LessonProgress>()
                .HasIndex(lp => new { lp.EnrollmentId, lp.LessonId }).IsUnique();

            modelBuilder.Entity<Certificate>()
                .HasIndex(c => c.EnrollmentId).IsUnique();

            modelBuilder.Entity<Review>()
                .HasIndex(r => new { r.StudentId, r.TargetId, r.TargetType }).IsUnique();

            modelBuilder.Entity<RefreshToken>()
                .HasIndex(rt => rt.Token).IsUnique();

            // ── Enum to string conversions ────────────────────────────
            modelBuilder.Entity<User>()
                .Property(u => u.Role).HasConversion<string>();

            modelBuilder.Entity<User>()
                .Property(u => u.AuthProvider).HasConversion<string>();

            modelBuilder.Entity<Course>()
                .Property(c => c.Status).HasConversion<string>();

            modelBuilder.Entity<Course>()
                .Property(c => c.Level).HasConversion<string>();

            modelBuilder.Entity<Lesson>()
                .Property(l => l.ContentType).HasConversion<string>();

            modelBuilder.Entity<Enrollment>()
                .Property(e => e.Status).HasConversion<string>();

            modelBuilder.Entity<Review>()
                .Property(r => r.TargetType).HasConversion<string>();

            modelBuilder.Entity<Question>()
                .Property(q => q.Type).HasConversion<string>();

            // ── Decimal precision ─────────────────────────────────────
            modelBuilder.Entity<Course>()
                .Property(c => c.Price).HasPrecision(10, 2);

            modelBuilder.Entity<Enrollment>()
                .Property(e => e.PaidPrice).HasPrecision(10, 2);

            // ── Always last ───────────────────────────────────────────
            base.OnModelCreating(modelBuilder);
        }

        public override Task<int> SaveChangesAsync(CancellationToken ct = default)
        {
            foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
            {
                if (entry.State == EntityState.Added)
                    entry.Entity.CreatedAt = DateTime.UtcNow;

                if (entry.State is EntityState.Added or EntityState.Modified)
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
            }

            return base.SaveChangesAsync(ct);
        }
    }
}
