using LMS.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.DTOs.Course
{
    public class CourseDto
    {

        public Guid Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public Guid CategoryId { get; set; }
        public string InstructorName { get; set; } = string.Empty;
        public string? ThumbnailUrl { get; set; }
        public decimal Price { get; set; }
        public int SectionCount { get; set; }
        public int LessonCount { get; set; }

        public string Language { get; set; } = string.Empty;

        public string Status { get; set; }
    }
}
