using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.DTOs
{
    public sealed record EnrollmentDetailDto(
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
    string? CertificateUrl,
    DateTime EnrolledAt,
    DateTime? CompletedAt,
    IEnumerable<SectionProgressDto> Sections);

    public sealed record SectionProgressDto(
        Guid SectionId,
        string Title,
        int OrderIndex,
        IEnumerable<LessonProgressDto> Lessons);

    public sealed record LessonProgressDto(
        Guid LessonId,
        string Title,
        string ContentType,
        int DurationSeconds,
        bool IsCompleted,
        int WatchedSeconds,
        bool IsFreePreview);
}
