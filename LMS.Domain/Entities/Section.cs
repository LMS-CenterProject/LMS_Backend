using LMS.Domain.Primitives;
using System;
using System.Collections.Generic;
using System.Text;
using static LMS.Domain.Errors.DomainErrors;

namespace LMS.Domain.Entities
{
    public sealed class Section : Entity
    {
        private Section() { }

        public Guid CourseId { get; private set; }
        public string Title { get; private set; } = string.Empty;
        public int OrderIndex { get; private set; }

        // Navigation properties
        public Course Course { get; private set; } = null!;
        public ICollection<Lesson> Lessons { get; private set; } = [];
        public bool IsDeleted { get; private set; } = false;


        private Section(Guid courseId, string title, int orderIndex)
        {
            CourseId = courseId;
            Title = title.Trim();
            OrderIndex = orderIndex;
        }

        // ── Factory method ───────────────────────────────────────
        public static Section Create(Guid courseId, string title, int orderIndex)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Section title cannot be empty.", nameof(title));

            if (orderIndex < 0)
                throw new ArgumentException("OrderIndex must be greater than or equal to 0.", nameof(orderIndex));

            return new Section(courseId, title, orderIndex);
        }

        // ── Business methods ─────────────────────────────────────

        public void Update(string title, int orderIndex)
        {
            Title = title.Trim();
            OrderIndex = orderIndex;
        }

        // Soft delete method
        public void SoftDelete()
        {
            if (IsDeleted)
                throw new InvalidOperationException("Course already deleted.");

            IsDeleted = true;
        }
    }
}
