using LMS.Application.Common.Interfaces;
using LMS.Application.Common.Models;
using LMS.Application.DTOs;
using LMS.Domain.Entities;
using LMS.Domain.Errors;
using LMS.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Progress.Commands.UpdateLessonProgress
{
    public sealed class UpdateLessonProgressCommandHandler(
    IUnitOfWork uow,
    ICurrentUserService current)
    : IRequestHandler<UpdateLessonProgressCommand, Result<UpdateProgressResultDto>>
    {
        public async Task<Result<UpdateProgressResultDto>> Handle(
            UpdateLessonProgressCommand cmd, CancellationToken ct)
        {
            var studentId = current.UserId!.Value;

            // 1. Get the lesson with its section to know the courseId
            var lesson = await uow.Lessons.GetByIdWithSectionAsync(cmd.LessonId, ct);
            if (lesson is null)
                return Result.Failure<UpdateProgressResultDto>(DomainErrors.Lesson.NotFound);

            var courseId = lesson.Section.CourseId;

            // 2. Get the enrollment — student must be enrolled
            var enrollment = await uow.Enrollments
                .GetByStudentAndCourseAsync(studentId, courseId, ct);

            if (enrollment is null || !enrollment.IsActive)
                return Result.Failure<UpdateProgressResultDto>(
                    DomainErrors.LessonProgress.NotEnrolled);

            // 3. Get or create the LessonProgress record
            var progress = await uow.LessonProgresses
                .GetByEnrollmentAndLessonAsync(enrollment.Id, cmd.LessonId, ct);

            if (progress is null)
            {
                progress = Domain.Entities.LessonProgress.Create(enrollment.Id, cmd.LessonId);
                await uow.LessonProgresses.AddAsync(progress, ct);
            }

            // 4. Update watched seconds
            var previouslyCompleted = progress.IsCompleted;
            progress.UpdateProgress(cmd.WatchedSeconds);

            // 5. Mark complete if requested and not already done
            if (cmd.IsCompleted && !previouslyCompleted)
            {
                progress.MarkAsCompleted();

                // Add the lesson duration to enrollment's total watched time
                enrollment.AddWatchedSeconds(lesson.DurationSeconds);
            }

            // 6. Check if all lessons in the course are now complete
            var totalLessons = await uow.Lessons.CountByCourseAsync(courseId, ct);

            // Count after saving progress (include the one we just marked)
            var completedCount = await uow.LessonProgresses.CountCompletedAsync(enrollment.Id, ct);

            // Add 1 if we just marked this one complete but haven't saved yet
            if (cmd.IsCompleted && !previouslyCompleted)
                completedCount += 1;

            var enrollmentCompleted = false;

            if (completedCount >= totalLessons && !enrollment.IsCompleted)
            {
                // All lessons done — complete the enrollment
                // This raises EnrollmentCompletedEvent internally
                enrollment.Complete();
                enrollmentCompleted = true;
            }

            // 7. Save everything — UnitOfWork dispatches domain events after save
            await uow.SaveChangesAsync(ct);

            var progressPercentage = totalLessons == 0
                ? 0.0
                : Math.Round((double)completedCount / totalLessons * 100, 1);

            return Result.Success(new UpdateProgressResultDto(
                LessonId: cmd.LessonId,
                IsCompleted: progress.IsCompleted,
                WatchedSeconds: progress.WatchedSeconds,
                CourseProgressPercentage: progressPercentage,
                EnrollmentCompleted: enrollmentCompleted,
                CertificateIssued: enrollmentCompleted));
        }
    }
}
