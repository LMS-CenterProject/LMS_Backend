using LMS.Application.Common.Interfaces;
using LMS.Application.Common.Models;
using LMS.Application.DTOs;
using LMS.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Certificates.Queries.GetMyCertificates
{
    public sealed class GetMyCertificatesQueryHandler(
    IUnitOfWork uow,
    ICurrentUserService current)
    : IRequestHandler<GetMyCertificatesQuery, Result<IEnumerable<CertificateDto>>>
    {
        public async Task<Result<IEnumerable<CertificateDto>>> Handle(
            GetMyCertificatesQuery query, CancellationToken ct)
        {
            var studentId = current.UserId!.Value;
            var certificates = await uow.Certificates.GetByStudentAsync(studentId, ct);

            var dtos = certificates.Select(c => new CertificateDto(
                Id: c.Id,
                EnrollmentId: c.EnrollmentId,
                CourseId: c.Enrollment.CourseId,
                CourseTitle: c.Enrollment.Course.Title,
                InstructorName: c.Enrollment.Course.Instructor.FullName,
                CertificateUrl: c.CertificateUrl,
                IssuedAt: c.IssuedAt));

            return Result.Success(dtos);
        }
    }
}
