using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.DTOs
{
    public sealed record CertificateDto(
    Guid Id,
    Guid EnrollmentId,
    Guid CourseId,
    string CourseTitle,
    string InstructorName,
    string CertificateUrl,
    DateTime IssuedAt);
}
