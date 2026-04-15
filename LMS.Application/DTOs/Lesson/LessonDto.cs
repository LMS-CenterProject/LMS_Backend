using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.DTOs.Lesson
{
    public class LessonDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public Guid SectionId { get; set; }
        public string ContentUrl { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public int DurationSeconds { get; set; }
        public int OrderIndex { get; set; }
        public bool IsFreePreview { get; set; }
    }
}
