using LMS.Application.DTOs.Answers;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.DTOs.Question
{
    public class StudentQuestionWithAnswerDto
    {
        public Guid QuestionId { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public Guid AnswerId { get; set; }
        public string AnswerText { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }  // was the chosen answer correct?
    }
}
