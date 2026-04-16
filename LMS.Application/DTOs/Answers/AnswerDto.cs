using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.DTOs.Answers
{
    public class AnswerDto
    {
        public Guid Id { get; set; }
        public string Text { get; set; } = string.Empty;
    }
}
