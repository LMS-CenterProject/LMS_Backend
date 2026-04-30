using LMS.Application.DTOs.Quize;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Quiz.Query.GetMyAttemp
{
    public record GetMyAttemptsQuery() : IRequest<List<QuizAttemptDto>>;
}
