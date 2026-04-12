using LMS.Domain.Events;
using LMS.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Certificates.EventHandlers
{
    public sealed class EnrollmentCompletedEventHandler(IUnitOfWork uow)
        : INotificationHandler<EnrollmentCompletedEvent>
    {
        public async Task Handle(
            EnrollmentCompletedEvent notification,
            CancellationToken ct)
        {
            // Guard: don't issue duplicate certificate
            var alreadyExists = await uow.Certificates
                .ExistsForEnrollmentAsync(notification.EnrollmentId, ct);

            if (alreadyExists)
            {
                return;
            }

            // Generate certificate URL
            // In production: call a PDF generation service and upload to blob storage
            var certificateUrl = GenerateCertificateUrl(
                notification.EnrollmentId, notification.StudentId, notification.CourseId);

            var certificate = Domain.Entities.Certificate.Issue(notification.EnrollmentId, certificateUrl);

            await uow.Certificates.AddAsync(certificate, ct);
            await uow.SaveChangesAsync(ct);
        }

        private static string GenerateCertificateUrl(
            Guid enrollmentId, Guid studentId, Guid courseId) =>
            $"https://storage.lms.com/certificates/{courseId}/{studentId}/{enrollmentId}.pdf";
    }
}
