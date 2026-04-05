using LMS.Domain.Enums;
using LMS.Domain.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Domain.Entities
{
    public sealed class Review : AuditableEntity
    {
        private Review() { }

        public Guid StudentId { get; private set; }
        public Guid TargetId { get; private set; }
        public ReviewTargetType TargetType { get; private set; }
        public Guid EligibilityId { get; private set; }
        public byte Rating { get; private set; }
        public string? Comment { get; private set; }

        // Navigation property
        public User Student { get; private set; } = null!;

        // ── Computed ─────────────────────────────────────────────

        public bool IsValidRating => Rating is >= 1 and <= 5;

        // ── Factory method ───────────────────────────────────────

        public static Review Create(
            Guid studentId,
            Guid targetId,
            ReviewTargetType targetType,
            Guid eligibilityId,
            byte rating,
            string? comment)
        {
            if (rating is < 1 or > 5)
                throw new ArgumentOutOfRangeException(nameof(rating), "Rating must be between 1 and 5.");

            return new Review
            {
                StudentId = studentId,
                TargetId = targetId,
                TargetType = targetType,
                EligibilityId = eligibilityId,
                Rating = rating,
                Comment = comment?.Trim()
            };
        }

        // ── Business methods ─────────────────────────────────────

        public void Update(byte rating, string? comment)
        {
            if (rating is < 1 or > 5)
                throw new ArgumentOutOfRangeException(nameof(rating), "Rating must be between 1 and 5.");

            Rating = rating;
            Comment = comment?.Trim();
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
