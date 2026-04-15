
using LMS.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.DTOs.Course
{
    public class CourseDetailDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public CourseStatus Status { get; set; }
        public CourseLevel Level { get; set; }
        public string Language { get; set; } = string.Empty;
        public string? ThumbnailUrl { get; set; }
        public int TotalWatchSeconds { get; set; }

        public Guid InstructorId { get; set; }
        public string InstructorName { get; set; } = string.Empty;

        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;

        public List<SectionDto> Sections { get; set; } = new();
    }
}
