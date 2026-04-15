using LMS.Application.Common.Interfaces;
using LMS.Application.Common.Models;
using LMS.Application.DTOs;
using LMS.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Enrollments.Queries.GetMyEnrollments
{
    public sealed class GetMyEnrollmentsQueryHandler(
    IUnitOfWork uow,
    ICurrentUserService current)
    : IRequestHandler<GetMyEnrollmentsQuery, Result<IEnumerable<EnrollmentDto>>>
    {
        public async Task<Result<IEnumerable<EnrollmentDto>>> Handle(
            GetMyEnrollmentsQuery query, CancellationToken ct)
        {
            var studentId = current.UserId!.Value;
            var enrollments = await uow.Enrollments.GetByStudentAsync(studentId, ct);

            var dtos = enrollments.Select(e =>
            {
                // Total course duration = sum of all lesson durations
                var totalDuration = e.Course.Sections
                    .SelectMany(s => s.Lessons)
                    .Sum(l => l.DurationSeconds);

                // Completed lessons across all sections
                var completedLessons = e.LessonProgresses
                    .Count(lp => lp.IsCompleted);

                // Total lessons in the course
                var totalLessons = e.Course.Sections
                    .SelectMany(s => s.Lessons)
                    .Count();

                // Progress as percentage
                var progress = totalLessons == 0
                    ? 0.0
                    : Math.Round((double)completedLessons / totalLessons * 100, 1);

                return new EnrollmentDto(
                    Id: e.Id,
                    CourseId: e.CourseId,
                    CourseTitle: e.Course.Title,
                    InstructorName: e.Course.Instructor.FullName,
                    ThumbnailUrl: e.Course.ThumbnailUrl,
                    PaidPrice: e.PaidPrice,
                    Status: e.Status.ToString(),
                    TotalWatchedSeconds: e.TotalWatchedSeconds,
                    TotalCourseDurationSeconds: totalDuration,
                    ProgressPercentage: progress,
                    HasCertificate: e.Certificate is not null,
                    EnrolledAt: e.EnrolledAt,
                    CompletedAt: e.CompletedAt);
            });

            return Result.Success(dtos);
        }
    }
}
