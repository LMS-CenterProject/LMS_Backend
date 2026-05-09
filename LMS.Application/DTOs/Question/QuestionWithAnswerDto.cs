using LMS.Application.DTOs.Answers;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.DTOs.Question
{
    public class QuestionWithAnswersDto
    {
        public Guid Id { get; set; }
        public Guid QuizId { get; set; }
        public string Text { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public int Points { get; set; }

        public List<AnswerDto> Answers { get; set; } = [];
    }
}
