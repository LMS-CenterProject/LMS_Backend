using LMS.Application.DTOs.Quize;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Quiz.Query.GetStudentAttemp
{
    public record GetStudentAttemptQuery()
    : IRequest<List<QuizAttemptDto>>;
}
