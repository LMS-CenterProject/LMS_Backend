using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.DTOs.Answers
{
    public class AnswerQuestionDto
    {
        public Guid Id { get; set; }
        public string? Text { get; set; }
        public bool IsCorrect { get; set; } = false;
    }
}
