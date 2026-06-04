using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.DTOs.Quiz
{
    public class QuizStatsDto
    {
        public Guid QuizId { get; set; }
        public int TotalAttempts { get; set; }
        public int PassedCount { get; set; }
        public int FailedCount { get; set; }
        public int AverageScore {get; set;}
    }
}
