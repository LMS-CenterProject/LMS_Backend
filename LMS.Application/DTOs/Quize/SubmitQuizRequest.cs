using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.DTOs.Quize
{
    public class SubmitQuizRequest
    {
        public Guid QuizId { get; set; }
        public Dictionary<Guid, List<Guid>> Answers { get; set; } = new();
    }
}
