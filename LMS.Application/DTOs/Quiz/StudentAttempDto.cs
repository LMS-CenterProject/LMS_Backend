using LMS.Application.DTOs.Question;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.DTOs.Quiz
{
    public class StudentAttempDto
    {
        public Guid AttemptId { get; set; }
        public Guid QuizId { get; set; }
        public string QuizTitle { get; set; } = string.Empty;
        public int Score { get; set; }
        public bool Passed { get; set; }
        public DateTime AttemptedAt { get; set; }
        public List<StudentQuestionWithAnswerDto> Questions { get; set; } = [];

    }
}
