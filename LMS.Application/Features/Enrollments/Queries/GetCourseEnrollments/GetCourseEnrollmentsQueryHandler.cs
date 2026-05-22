using LMS.Application.Common.Interfaces;
using LMS.Application.Common.Models;
using LMS.Application.DTOs;
using LMS.Domain.Enums;
using LMS.Domain.Errors;
using LMS.Domain.Interfaces;
using MediatR;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Enrollments.Queries.GetCourseEnrollments
{
    public sealed class GetCourseEnrollmentsQueryHandler(
       IUnitOfWork uow,
       ICurrentUserService current)
       : IRequestHandler<GetCourseEnrollmentsQuery, Result<CourseEnrollmentsDto>>
    {
        public async Task<Result<CourseEnrollmentsDto>> Handle(
            GetCourseEnrollmentsQuery query, CancellationToken ct)
        {
            // 1. Verify the course exists
            var course = await uow.Courses.GetByIdAsync(query.CourseId, ct);
            if (course is null)
                return Result.Failure<CourseEnrollmentsDto>(DomainErrors.Course.NotFound);

            // 2. Ownership check for Instructors
            //    Admins and SuperAdmins can see any course enrollments
            //    Instructors can only see enrollments for their own courses
            var callerRole = current.Role;
            var isAdmin = callerRole is "Admin" or "SuperAdmin";

            if (!isAdmin && course.InstructorId != current.UserId)
                return Result.Failure<CourseEnrollmentsDto>(DomainErrors.Course.Unauthorized);

            // 3. Fetch all enrollments for this course
            var enrollments = (await uow.Enrollments.GetByCourseAsync(query.CourseId, ct))
                .ToList();

            // 4. Map to DTOs — calculate progress per student
            var dtos = enrollments.Select(e =>
            {
                var totalLessons = e.Course.Sections
                    .SelectMany(s => s.Lessons)
                    .Count();

                var completedLessons = e.LessonProgresses
                    .Count(lp => lp.IsCompleted);

                var progress = totalLessons == 0
                    ? 0.0
                    : Math.Round((double)completedLessons / totalLessons * 100, 1);

                return new CourseStudentEnrollmentDto(
                    EnrollmentId: e.Id,
                    StudentId: e.StudentId,
                    StudentName: e.Student.FullName,
                    StudentEmail: e.Student.Email,
                    Status: e.Status.ToString(),
                    ProgressPercentage: progress,
                    CompletedLessons: completedLessons,
                    TotalLessons: totalLessons,
                    TotalWatchedSeconds: e.TotalWatchedSeconds,
                    HasCertificate: e.Certificate is not null,
                    EnrolledAt: e.EnrolledAt,
                    CompletedAt: e.CompletedAt);
            }).ToList();

            // 5. Build summary stats
            return Result.Success(new CourseEnrollmentsDto(
                CourseId: query.CourseId,
                CourseTitle: course.Title,
                TotalEnrollments: dtos.Count,
                CompletedEnrollments: dtos.Count(d => d.Status == EnrollmentStatus.Completed.ToString()),
                ActiveEnrollments: dtos.Count(d => d.Status == EnrollmentStatus.Active.ToString()),
                Enrollments: dtos));
        }
    }
}
