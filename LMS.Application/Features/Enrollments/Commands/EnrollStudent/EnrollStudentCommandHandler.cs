using LMS.Application.Common.Interfaces;
using LMS.Application.Common.Models;
using LMS.Domain.Entities;
using LMS.Domain.Enums;
using LMS.Domain.Errors;
using LMS.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Enrollments.Commands.EnrollStudent
{
    public sealed class EnrollStudentCommandHandler(
    IUnitOfWork uow,
    ICurrentUserService current)
    : IRequestHandler<EnrollStudentCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(
            EnrollStudentCommand cmd, CancellationToken ct)
        {
            var studentId = current.UserId!.Value;

            // 1. Check course exists and is published
            var course = await uow.Courses.GetByIdAsync(cmd.CourseId, ct);

            if (course is null)
                return Result.Failure<Guid>(DomainErrors.Course.NotFound);

            if (course.Status != CourseStatus.Published)
                return Result.Failure<Guid>(DomainErrors.Course.NotPublished);

            // 2. Check not already enrolled
            var alreadyEnrolled = await uow.Enrollments.IsEnrolledAsync(
                studentId, cmd.CourseId, ct);

            if (alreadyEnrolled)
                return Result.Failure<Guid>(DomainErrors.Enrollment.AlreadyExists);

            // 3. Create enrollment
            var enrollment = Enrollment.Create(studentId, cmd.CourseId, course.Price);
            await uow.Enrollments.AddAsync(enrollment, ct);
            await uow.SaveChangesAsync(ct);

            return Result.Success(enrollment.Id);
        }
    }
}
