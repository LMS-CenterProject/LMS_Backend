using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.DTOs
{
    public sealed record EnrollmentDto(
    Guid Id,
    Guid CourseId,
    string CourseTitle,
    string InstructorName,
    string? ThumbnailUrl,
    decimal PaidPrice,
    string Status,
    int TotalWatchedSeconds,
    int TotalCourseDurationSeconds,
    double ProgressPercentage,
    bool HasCertificate,
    DateTime EnrolledAt,
    DateTime? CompletedAt);
}
