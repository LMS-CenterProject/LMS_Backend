using LMS.Application.DTOs.Quiz;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Attemps.Query.GetStudentAttempByName
{
    public record GetStudentAttempByNameQuery(Guid QuizId) : IRequest<QuizStudentsStatsDto>;
}
