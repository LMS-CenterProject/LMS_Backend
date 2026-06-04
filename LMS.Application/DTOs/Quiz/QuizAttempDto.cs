using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.DTOs.Quiz
{
    public class QuizAttemptDto
    {
        public Guid Id { get; set; }
        public Guid QuizId { get; set; }
        public int Score { get; set; }
        public bool Passed { get; set; }
        public DateTime AttemptedAt { get; set; }
    }
}
