using LMS.Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Enrollments.Commands.EnrollStudent
{
    public sealed record EnrollStudentCommand(Guid CourseId)
    : IRequest<Result<Guid>>;
}
