using LMS.Domain.Enums;
using LMS.Domain.Events;
using LMS.Domain.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Domain.Entities
{
    public sealed class Enrollment : AuditableEntity
    {
        private Enrollment() { }

        public Guid StudentId { get; private set; }
        public Guid CourseId { get; private set; }
        public decimal PaidPrice { get; private set; }
        public EnrollmentStatus Status { get; private set; } = EnrollmentStatus.Active;
        public int TotalWatchedSeconds { get; private set; }
        public DateTime EnrolledAt { get; private set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; private set; }

        // Navigation properties
        public User Student { get; private set; } = null!;
        public Course Course { get; private set; } = null!;
        public Certificate? Certificate { get; private set; }
        public ICollection<LessonProgress> LessonProgresses { get; private set; } = [];

        // ── Computed ─────────────────────────────────────────────

        public bool IsActive => Status == EnrollmentStatus.Active;
        public bool IsCompleted => Status == EnrollmentStatus.Completed;

        // ── Factory method ───────────────────────────────────────

        public static Enrollment Create(Guid studentId, Guid courseId, decimal paidPrice)
        {
            return new Enrollment
            {
                StudentId = studentId,
                CourseId = courseId,
                PaidPrice = paidPrice
            };
        }

        // ── Business methods ─────────────────────────────────────

        public void AddWatchedSeconds(int seconds)
        {
            if (seconds <= 0) return;

            TotalWatchedSeconds += seconds;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Complete()
        {
            if (Status != EnrollmentStatus.Active)
                throw new InvalidOperationException("Only active enrollments can be completed.");

            Status = EnrollmentStatus.Completed;
            CompletedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;

            RaiseDomainEvent(new EnrollmentCompletedEvent(Id, StudentId, CourseId));
        }

        public void Refund()
        {
            if (Status == EnrollmentStatus.Refunded)
                throw new InvalidOperationException("Enrollment is already refunded.");

            Status = EnrollmentStatus.Refunded;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
