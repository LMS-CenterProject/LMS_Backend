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

        // ── Factory method ───────────────────────────────────────

        public static Section Create(Guid courseId, string title, int orderIndex) =>
            new() { CourseId = courseId, Title = title.Trim(), OrderIndex = orderIndex };

        // ── Business methods ─────────────────────────────────────

        public void Update(string title, int orderIndex)
        {
            Title = title.Trim();
            OrderIndex = orderIndex;
        }
    }
}
