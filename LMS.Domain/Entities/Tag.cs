using LMS.Domain.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Domain.Entities
{
    public sealed class Tag : Entity
    {
        private Tag() { }

        public string Name { get; private set; } = string.Empty;

        // Navigation property
        public ICollection<CourseTag> CourseTags { get; private set; } = [];

        // ── Factory method ───────────────────────────────────────

        public static Tag Create(string name) =>
            new() { Name = name.Trim().ToLowerInvariant() };

        // ── Business methods ─────────────────────────────────────

        public void Rename(string newName) => Name = newName.Trim().ToLowerInvariant();
    }
}
