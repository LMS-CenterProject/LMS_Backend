using LMS.Domain.Enums;
using LMS.Domain.Events;
using LMS.Domain.Primitives;
using System;
using System.Collections.Generic;
using System.Text;
using static LMS.Domain.Errors.DomainErrors;
using static System.Collections.Specialized.BitVector32;

namespace LMS.Domain.Entities
{
    public sealed class Course : AuditableEntity
    {
        private Course() { }

        public Guid InstructorId { get; private set; }
        public Guid CategoryId { get; private set; }
        public string Title { get; private set; } = string.Empty;
        public string? Description { get; private set; }
        public decimal Price { get; private set; }
        public CourseStatus Status { get; private set; } = CourseStatus.Draft;
        public CourseLevel Level { get; private set; } = CourseLevel.All;
        public string Language { get; private set; } = "English";
        public string? ThumbnailUrl { get; private set; }
        public int TotalWatchSeconds { get; private set; }
        public bool IsDeleted { get; private set; } = false;
        public DateTime? DeletedAt { get; private set; }

        // Navigation properties
        public User Instructor { get; private set; } = null!;
        public Category Category { get; private set; } = null!;
        public ICollection<Section> Sections { get; private set; } = [];
        public ICollection<Enrollment> Enrollments { get; private set; } = [];
        public ICollection<Quiz> Quizzes { get; private set; } = [];
        public ICollection<CourseTag> CourseTags { get; private set; } = [];

        // ── Computed ─────────────────────────────────────────────

        public bool IsOwner(Guid userId) => InstructorId == userId;

        // ── Factory method ───────────────────────────────────────

        public static Course Create(
            Guid instructorId,
            Guid categoryId,
            string title,
            string? description,
            decimal price,
            CourseLevel level,
            string language)
        {
            return new Course
            {
                InstructorId = instructorId,
                CategoryId = categoryId,
                Title = title.Trim(),
                Description = description?.Trim(),
                Price = price,
                Level = level,
                Language = language.Trim()
            };
        }

        // ── Business methods ─────────────────────────────────────

        public void Update(
            string title,
            string? description,
            decimal price,
            CourseLevel level,
            string language,
            string? thumbnailUrl,
            Guid categoryId)
        {
            Title = title.Trim();
            Description = description?.Trim();
            Price = price;
            Level = level;
            Language = language.Trim();
            ThumbnailUrl = thumbnailUrl;
            CategoryId = categoryId;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Publish()
        {
            if (Status == CourseStatus.Published)
                throw new InvalidOperationException("Course is already published.");

            if (Status == CourseStatus.Archived)
                throw new InvalidOperationException("Cannot publish an archived course.");

            Status = CourseStatus.Published;
            UpdatedAt = DateTime.UtcNow;

            RaiseDomainEvent(new CoursePublishedEvent(Id, InstructorId));
        }

        public void Archive()
        {
            if (Status == CourseStatus.Archived)
                throw new InvalidOperationException("Course is already archived.");

            Status = CourseStatus.Archived;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Unpublish()
        {
            if (Status != CourseStatus.Published)
                throw new InvalidOperationException("Only published courses can be unpublished.");

            Status = CourseStatus.Draft;
            UpdatedAt = DateTime.UtcNow;
        }

        public void RecalculateTotalWatchSeconds(int totalSeconds)
        {
            TotalWatchSeconds = totalSeconds;
            UpdatedAt = DateTime.UtcNow;
        }
        // Soft delete method
        public void SoftDelete()
        {
            if (IsDeleted)
                throw new InvalidOperationException("Course already deleted.");

            IsDeleted = true;
            DeletedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
