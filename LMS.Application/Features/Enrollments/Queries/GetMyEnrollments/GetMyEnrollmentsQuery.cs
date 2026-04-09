using LMS.Application.Common.Models;
using LMS.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Enrollments.Queries.GetMyEnrollments
{
    public sealed record GetMyEnrollmentsQuery : IRequest<Result<IEnumerable<EnrollmentDto>>>;
}
