using LMS.Application.DTOs.Quiz;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Attemps.Query.GetAttempForStudent
{
    public record GetAttempForStudentQuery(Guid QuizId) : IRequest<QuizStatsDto>;
}
