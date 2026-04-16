using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.DTOs.Submit
{
    public class SubmitDto
    {
        public Guid QuestionId { get; set; }
        public List<Guid> SelectedAnswerIds { get; set; } = [];
    }
}
