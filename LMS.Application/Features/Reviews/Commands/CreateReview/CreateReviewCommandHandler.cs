using LMS.Application.Common.Interfaces;
using LMS.Application.Common.Models;
using LMS.Domain.Enums;
using LMS.Domain.Errors;
using LMS.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Reviews.Commands.CreateReview
{
    public sealed class CreateReviewCommandHandler(
        IUnitOfWork uow,
        ICurrentUserService current)
        : IRequestHandler<CreateReviewCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(
            CreateReviewCommand cmd, CancellationToken ct)
        {
            var studentId = current.UserId!.Value;

            // 1. Check not already reviewed
            var alreadyReviewed = await uow.Reviews.ExistsAsync(
                studentId, cmd.TargetId, cmd.TargetType, ct);

            if (alreadyReviewed)
                return Result.Failure<Guid>(DomainErrors.Review.AlreadyExists);

            // 2. Eligibility check — different rule per TargetType
            var eligibilityResult = cmd.TargetType switch
            {
                ReviewTargetType.Course => await CheckCourseEligibilityAsync(studentId, cmd.TargetId, ct),
                ReviewTargetType.Teacher => await CheckTeacherEligibilityAsync(studentId, cmd.TargetId, ct),
                ReviewTargetType.Lesson => await CheckLessonEligibilityAsync(studentId, cmd.TargetId, ct),
                _ => (null, DomainErrors.Review.NotEligible)
            };

            if (eligibilityResult.Item1 is null)
                return Result.Failure<Guid>(eligibilityResult.Item2);

            // 3. Cannot review own course
            if (cmd.TargetType == ReviewTargetType.Course)
            {
                var course = await uow.Courses.GetByIdAsync(cmd.TargetId, ct);
                if (course?.InstructorId == studentId)
                    return Result.Failure<Guid>(DomainErrors.Review.CannotReviewOwn);
            }

            // 4. Create and save
            var review = Domain.Entities.Review.Create(
                studentId: studentId,
                targetId: cmd.TargetId,
                targetType: cmd.TargetType,
                eligibilityId: eligibilityResult.Item1.Value,
                rating: cmd.Rating,
                comment: cmd.Comment);

            await uow.Reviews.AddAsync(review, ct);
            await uow.SaveChangesAsync(ct);

            return Result.Success(review.Id);
        }

        // ── Eligibility helpers ───────────────────────────────────

        // Course: student must have an active enrollment in that course
        private async Task<(Guid?, LMS.Domain.Errors.Error)> CheckCourseEligibilityAsync(
            Guid studentId, Guid courseId, CancellationToken ct)
        {
            var enrollment = await uow.Enrollments
                .GetByStudentAndCourseAsync(studentId, courseId, ct);

            return enrollment?.IsActive == true
                ? (enrollment.Id, DomainErrors.Review.NotEligible)
                : ((Guid?)null, DomainErrors.Review.NotEligible);
        }

        // Teacher: student must be enrolled in at least one of this teacher's courses
        private async Task<(Guid?, LMS.Domain.Errors.Error)> CheckTeacherEligibilityAsync(
            Guid studentId, Guid teacherId, CancellationToken ct)
        {
            var enrollments = await uow.Enrollments.GetByStudentAsync(studentId, ct);

            var qualifying = enrollments
                .FirstOrDefault(e => e.Course.InstructorId == teacherId && e.IsActive);

            return qualifying is not null
                ? (qualifying.Id, DomainErrors.Review.NotEligible)
                : ((Guid?)null, DomainErrors.Review.NotEligible);
        }

        // Lesson: student must have completed the lesson
        private async Task<(Guid?, LMS.Domain.Errors.Error)> CheckLessonEligibilityAsync(
            Guid studentId, Guid lessonId, CancellationToken ct)
        {
            var enrollments = await uow.Enrollments.GetByStudentAsync(studentId, ct);

            foreach (var enrollment in enrollments)
            {
                var progress = enrollment.LessonProgresses
                    .FirstOrDefault(lp => lp.LessonId == lessonId && lp.IsCompleted);

                if (progress is not null)
                    return (progress.Id, DomainErrors.Review.LessonNotCompleted);
            }

            return ((Guid?)null, DomainErrors.Review.LessonNotCompleted);
        }
    }   
}
