using LMS.Application.DTOs.Quiz;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Quiz.Query.GetCourseQuiz
{
    public record GetCourseQuizzesQuery(Guid CourseId)
    : IRequest<List<QuizWithQuestionDto>>;
}
