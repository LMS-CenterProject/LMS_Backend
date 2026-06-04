using LMS.Application.Features.Courses.Commands.PublishCourse;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.DTOs.Quiz
{
    public class StudentAttemptSummaryDto
    {
        public Guid StudentId { get; set; }
        public string? StudentName { get; set; } 
        public int TotalAttempts { get; set; }
        public int BestScore { get; set; }
        public bool Passed { get; set; }
        public DateTime LastAttempt { get; set; }


    }
    public class QuizStudentsStatsDto
    {
        public Guid QuizId { get; set; }
        public int TotalStudents { get; set; }
        public int TotalAttempts { get; set; }
        public List<StudentAttemptSummaryDto> Students { get; set; } = [];
    }
}
