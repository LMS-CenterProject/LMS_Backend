using LMS.Application.DTOs.Quiz;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Quiz.Query.GetMyAttemp
{
    public record GetMyAttemptsQuery() : IRequest<List<StudentAttempDto>>;
}
