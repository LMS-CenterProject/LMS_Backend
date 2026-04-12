using LMS;
using LMS.Application.Features.Certificates;
using LMS.Application.Features.Certificates.EventHandlers;
using LMS.Domain;
using LMS.Domain.Entities;
using LMS.Domain.Events;
using LMS.Domain.Interfaces;
using LMS.Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;


namespace LMS.Application.Features.Certificates.EventHandlers.UnitTests
{
    [TestClass]
    public sealed class EnrollmentCompletedEventHandlerTests
    {
        /// <summary>
        /// Verifies that when a certificate already exists for the enrollment, the handler returns early
        /// and does not call AddAsync or SaveChangesAsync.
        /// Input: ExistsForEnrollmentAsync returns true for given EnrollmentId.
        /// Expected: No AddAsync or SaveChangesAsync calls; ExistsForEnrollmentAsync is awaited.
        /// </summary>
        [TestMethod]
        public async Task Handle_WhenCertificateAlreadyExists_DoesNotAddOrSaveAsync()
        {
            // Arrange
            var enrollmentId = Guid.NewGuid();
            var studentId = Guid.NewGuid();
            var courseId = Guid.NewGuid();

            var certRepoMock = new Mock<ICertificateRepository>(MockBehavior.Strict);
            certRepoMock
                .Setup(r => r.ExistsForEnrollmentAsync(enrollmentId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true)
                .Verifiable();

            // AddAsync and other calls should not occur
            certRepoMock
                .Setup(r => r.AddAsync(It.IsAny<Certificate>(), It.IsAny<CancellationToken>()))
                .Throws(new Exception("AddAsync should not be called"));

            var uowMock = new Mock<IUnitOfWork>(MockBehavior.Strict);
            uowMock.SetupGet(u => u.Certificates).Returns(certRepoMock.Object);

            var handler = new EnrollmentCompletedEventHandler(uowMock.Object);
            var notification = new EnrollmentCompletedEvent(enrollmentId, studentId, courseId);

            // Act
            await handler.Handle(notification, CancellationToken.None);

            // Assert
            certRepoMock.Verify(r => r.ExistsForEnrollmentAsync(enrollmentId, It.IsAny<CancellationToken>()), Times.Once);
            certRepoMock.Verify(r => r.AddAsync(It.IsAny<Certificate>(), It.IsAny<CancellationToken>()), Times.Never);
            uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        /// <summary>
        /// Verifies that when no certificate exists, the handler generates the expected certificate URL,
        /// issues a Certificate entity, calls AddAsync with that Certificate, and calls SaveChangesAsync.
        /// Input: Multiple test cases including Guid.Empty and randomized GUIDs to exercise URL formatting.
        /// Expected: AddAsync invoked once with Certificate matching EnrollmentId and expected CertificateUrl,
        /// and SaveChangesAsync invoked once.
        /// </summary>
        [TestMethod]
        public async Task Handle_WhenNoExistingCertificate_AddsCertificateAndSaves_ForVariousGuidCombinationsAsync()
        {
            // Arrange multiple cases to simulate parameterized behavior
            var testCases = new[]
            {
                // Normal random GUIDs
                (Enrollment: Guid.NewGuid(), Student: Guid.NewGuid(), Course: Guid.NewGuid()),
                // All empty GUIDs - edge case for URL formatting
                (Enrollment: Guid.Empty, Student: Guid.Empty, Course: Guid.Empty),
                // Identical GUIDs - ensure ordering in URL is as implemented
                (Enrollment: Guid.Parse("11111111-1111-1111-1111-111111111111"),
                 Student: Guid.Parse("11111111-1111-1111-1111-111111111111"),
                 Course: Guid.Parse("11111111-1111-1111-1111-111111111111"))
            };

            foreach (var (Enrollment, Student, Course) in testCases)
            {
                var certRepoMock = new Mock<ICertificateRepository>(MockBehavior.Strict);
                certRepoMock
                    .Setup(r => r.ExistsForEnrollmentAsync(Enrollment, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(false)
                    .Verifiable();

                certRepoMock
                    .Setup(r => r.AddAsync(It.Is<Certificate>(c =>
                        c != null &&
                        c.EnrollmentId == Enrollment &&
                        c.CertificateUrl == $"https://storage.lms.com/certificates/{Course}/{Student}/{Enrollment}.pdf"
                    ), It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask)
                    .Verifiable();

                var uowMock = new Mock<IUnitOfWork>(MockBehavior.Strict);
                uowMock.SetupGet(u => u.Certificates).Returns(certRepoMock.Object);
                uowMock
                    .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(1)
                    .Verifiable();

                var handler = new EnrollmentCompletedEventHandler(uowMock.Object);
                var notification = new EnrollmentCompletedEvent(Enrollment, Student, Course);

                // Act
                await handler.Handle(notification, CancellationToken.None);

                // Assert
                certRepoMock.Verify(r => r.ExistsForEnrollmentAsync(Enrollment, It.IsAny<CancellationToken>()), Times.Once);
                certRepoMock.Verify(r => r.AddAsync(It.IsAny<Certificate>(), It.IsAny<CancellationToken>()), Times.Once);
                uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

                // Cleanup/verifications for this iteration
                certRepoMock.VerifyAll();
                uowMock.VerifyAll();
            }
        }

    }
}