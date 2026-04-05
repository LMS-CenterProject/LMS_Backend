using LMS.Domain.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Domain.Entities
{
    public sealed class LessonProgress : Entity
    {
        private LessonProgress() { }

        public Guid EnrollmentId { get; private set; }
        public Guid LessonId { get; private set; }
        public bool IsCompleted { get; private set; }
        public int WatchedSeconds { get; private set; }
        public DateTime? LastWatchedAt { get; private set; }

        // Navigation properties
        public Enrollment Enrollment { get; private set; } = null!;
        public Lesson Lesson { get; private set; } = null!;

        // ── Factory method ───────────────────────────────────────

        public static LessonProgress Create(Guid enrollmentId, Guid lessonId) =>
            new() { EnrollmentId = enrollmentId, LessonId = lessonId };

        // ── Business methods ─────────────────────────────────────

        public void UpdateProgress(int watchedSeconds)
        {
            if (watchedSeconds < 0) return;

            WatchedSeconds = watchedSeconds;
            LastWatchedAt = DateTime.UtcNow;
        }

        public void MarkAsCompleted()
        {
            IsCompleted = true;
            LastWatchedAt = DateTime.UtcNow;
        }

        public void MarkAsIncomplete()
        {
            IsCompleted = false;
        }
    }
}
