using LMS.Application.DTOs.Question;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.DTOs.Quiz
{
    public class QuizDto
    {
        public Guid Id { get; set; }
        public Guid CourseId { get; set; }
        public string Title { get; set; } = string.Empty;
        public int PassScore { get; set; }
        public int? TimeLimitMinutes { get; set; }

        public List<QuestionDto> Questions { get; set; } = [];
    }
}
