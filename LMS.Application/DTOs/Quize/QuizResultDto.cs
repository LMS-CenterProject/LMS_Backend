using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.DTOs.Quize
{
    public class QuizResultDto
    {
        public Guid AttemptId { get; set; }
        public int Score { get; set; }
        public bool Passed { get; set; }
        public int RemainingAttempts { get; set; }
    }
}
