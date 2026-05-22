using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.DTOs
{
    // Summary returned per student enrollment
    public sealed record CourseStudentEnrollmentDto(
        Guid EnrollmentId,
        Guid StudentId,
        string StudentName,
        string StudentEmail,
        string Status,
        double ProgressPercentage,
        int CompletedLessons,
        int TotalLessons,
        int TotalWatchedSeconds,
        bool HasCertificate,
        DateTime EnrolledAt,
        DateTime? CompletedAt);

    // Wrapper with course-level stats
    public sealed record CourseEnrollmentsDto(
        Guid CourseId,
        string CourseTitle,
        int TotalEnrollments,
        int CompletedEnrollments,
        int ActiveEnrollments,
        IEnumerable<CourseStudentEnrollmentDto> Enrollments);
}

