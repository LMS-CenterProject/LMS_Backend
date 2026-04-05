using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Domain.Entities
{
    public sealed class CourseTag
    {
        public Guid CourseId { get; set; }
        public Guid TagId { get; set; }

        // Navigation properties
        public Course Course { get; set; } = null!;
        public Tag Tag { get; set; } = null!;
    }
}
