using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.DTOs
{
    public sealed record UpdateProgressResultDto(
    Guid LessonId,
    bool IsCompleted,
    int WatchedSeconds,
    double CourseProgressPercentage,
    bool EnrollmentCompleted,
    bool CertificateIssued);
}
