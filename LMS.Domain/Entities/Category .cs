using LMS.Domain.Primitives;
using System;
using System.Collections.Generic;
using System.Text;
using static LMS.Domain.Errors.DomainErrors;

namespace LMS.Domain.Entities
{
    public sealed class Category : Entity
    {
        private Category() { }

        public string Name { get; private set; } = string.Empty;
        public Guid? ParentCategoryId { get; private set; }

        // Navigation properties
        public Category? ParentCategory { get; private set; }
        public ICollection<Category> SubCategories { get; private set; } = [];
        public ICollection<Course> Courses { get; private set; } = [];

        // ── Factory method ───────────────────────────────────────

        public static Category Create(string name, Guid? parentCategoryId = null) =>
            new() { Name = name.Trim(), ParentCategoryId = parentCategoryId };

        // ── Business methods ─────────────────────────────────────

        public void Rename(string newName) => Name = newName.Trim();
    }
}
