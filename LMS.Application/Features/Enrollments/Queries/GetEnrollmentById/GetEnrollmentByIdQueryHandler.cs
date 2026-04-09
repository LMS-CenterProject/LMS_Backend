using LMS.Application.Common.Interfaces;
using LMS.Application.Common.Models;
using LMS.Application.DTOs;
using LMS.Domain.Errors;
using LMS.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Enrollments.Queries.GetEnrollmentById
{
    public sealed class GetEnrollmentByIdQueryHandler(
    IUnitOfWork uow,
    ICurrentUserService current)
    : IRequestHandler<GetEnrollmentByIdQuery, Result<EnrollmentDetailDto>>
    {
        public async Task<Result<EnrollmentDetailDto>> Handle(
            GetEnrollmentByIdQuery query, CancellationToken ct)
        {
            var enrollment = await uow.Enrollments.GetByIdWithDetailsAsync(
                query.EnrollmentId, ct);

            // 1. Not found
            if (enrollment is null)
                return Result.Failure<EnrollmentDetailDto>(DomainErrors.Enrollment.NotFound);

            // 2. Ownership check — students can only see their own enrollment
            if (enrollment.StudentId != current.UserId)
                return Result.Failure<EnrollmentDetailDto>(DomainErrors.Enrollment.Unauthorized);

            // 3. Build section + lesson progress tree
            var sections = enrollment.Course.Sections
                .OrderBy(s => s.OrderIndex)
                .Select(s => new SectionProgressDto(
                    SectionId: s.Id,
                    Title: s.Title,
                    OrderIndex: s.OrderIndex,
                    Lessons: s.Lessons
                        .OrderBy(l => l.OrderIndex)
                        .Select(l =>
                        {
                            // Find this student's progress record for this lesson
                            var progress = enrollment.LessonProgresses
                                .FirstOrDefault(lp => lp.LessonId == l.Id);

                            return new LessonProgressDto(
                                LessonId: l.Id,
                                Title: l.Title,
                                ContentType: l.ContentType.ToString(),
                                DurationSeconds: l.DurationSeconds,
                                IsCompleted: progress?.IsCompleted ?? false,
                                WatchedSeconds: progress?.WatchedSeconds ?? 0,
                                IsFreePreview: l.IsFreePreview);
                        })));

            // 4. Calculate totals
            var totalLessons = enrollment.Course.Sections
                .SelectMany(s => s.Lessons).Count();

            var completedLessons = enrollment.LessonProgresses
                .Count(lp => lp.IsCompleted);

            var progress = totalLessons == 0
                ? 0.0
                : Math.Round((double)completedLessons / totalLessons * 100, 1);

            var totalDuration = enrollment.Course.Sections
                .SelectMany(s => s.Lessons)
                .Sum(l => l.DurationSeconds);

            var dto = new EnrollmentDetailDto(
                Id: enrollment.Id,
                CourseId: enrollment.CourseId,
                CourseTitle: enrollment.Course.Title,
                InstructorName: enrollment.Course.Instructor.FullName,
                ThumbnailUrl: enrollment.Course.ThumbnailUrl,
                PaidPrice: enrollment.PaidPrice,
                Status: enrollment.Status.ToString(),
                TotalWatchedSeconds: enrollment.TotalWatchedSeconds,
                TotalCourseDurationSeconds: totalDuration,
                ProgressPercentage: progress,
                HasCertificate: enrollment.Certificate is not null,
                CertificateUrl: enrollment.Certificate?.CertificateUrl,
                EnrolledAt: enrollment.EnrolledAt,
                CompletedAt: enrollment.CompletedAt,
                Sections: sections);

            return Result.Success(dto);
        }
    }
}
