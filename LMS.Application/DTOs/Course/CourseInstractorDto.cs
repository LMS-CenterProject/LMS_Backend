using LMS.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.DTOs.Course
{
    public class CourseInstractorDto
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public Guid CategoryId { get; set; }
        public string? Thumbnail { get; set; }
        public string? CategoryName { get; set; }
        public decimal Price { get; set; }
        public string? Language { get; set; }
        public string? Status { get; set; }
    }
}
