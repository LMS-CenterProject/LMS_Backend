using LMS.Application.Common;
using LMS.Application.Common.Interfaces;
using LMS.Application.Common.Models;
using LMS.Application.Features.Enrollments.Commands.EnrollStudent;
using LMS.Domain;
using LMS.Domain.Entities;
using LMS.Domain.Enums;
using LMS.Domain.Errors;
using LMS.Domain.Interfaces;
using LMS.Domain.Interfaces.Repositories;
using LMS.Domain.Primitives;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;


namespace LMS.Application.Features.Enrollments.Commands.EnrollStudent.UnitTests
{
    [TestClass]
    public sealed class EnrollStudentCommandHandlerTests
    {
        /// <summary>
        /// Tests multiple failure scenarios produced by the handler:
        /// - Course not found -> returns DomainErrors.Course.NotFound
        /// - Course not published -> returns DomainErrors.Course.NotPublished
        /// - Already enrolled -> returns DomainErrors.Enrollment.AlreadyExists
        /// Input variations: course = null / course.Status != Published / IsEnrolledAsync == true
        /// Expected: Result is failure with the corresponding DomainErrors value.
        /// </summary>
        [TestMethod]
        public async Task Handle_InvalidCourseOrEnrollment_ReturnsExpectedFailure()
        {
            // Arrange common parts
            var studentId = Guid.NewGuid();
            var courseId = Guid.NewGuid();

            var ct = CancellationToken.None;

            var cmd = new EnrollStudentCommand(courseId);

            var uowMock = new Mock<IUnitOfWork>(MockBehavior.Strict);
            var coursesMock = new Mock<ICourseRepository>(MockBehavior.Strict);
            var enrollmentsMock = new Mock<IEnrollmentRepository>(MockBehavior.Strict);

            uowMock.SetupGet(u => u.Courses).Returns(coursesMock.Object);
            uowMock.SetupGet(u => u.Enrollments).Returns(enrollmentsMock.Object);

            var currentMock = new Mock<ICurrentUserService>(MockBehavior.Strict);
            currentMock.SetupGet(c => c.UserId).Returns((Guid?)studentId);

            var handler = new EnrollStudentCommandHandler(uowMock.Object, currentMock.Object);

            // Prepare scenarios
            var scenarios = new[]
            {
                new
                {
                    Name = "CourseNotFound",
                    Setup = new Action(() =>
                    {
                        coursesMock
                            .Setup(c => c.GetByIdAsync(courseId, ct))
                            .ReturnsAsync((Course?)null)
                            .Verifiable();
                        // Enrollments.IsEnrolledAsync should not be invoked in this scenario
                    }),
                    ExpectedError = DomainErrors.Course.NotFound
                },
                new
                {
                    Name = "CourseNotPublished",
                    Setup = new Action(() =>
                    {
                        var course = Course.Create(Guid.NewGuid(), Guid.NewGuid(), "t", null, 10m, CourseLevel.All, "en");
                        // course.Status defaults to Draft -> NotPublished scenario
                        coursesMock
                            .Setup(c => c.GetByIdAsync(courseId, ct))
                            .ReturnsAsync(course)
                            .Verifiable();
                    }),
                    ExpectedError = DomainErrors.Course.NotPublished
                },
                new
                {
                    Name = "AlreadyEnrolled",
                    Setup = new Action(() =>
                    {
                        var course = Course.Create(Guid.NewGuid(), Guid.NewGuid(), "t", null, 15m, CourseLevel.All, "en");
                        course.Publish(); // make it Published
                        coursesMock
                            .Setup(c => c.GetByIdAsync(courseId, ct))
                            .ReturnsAsync(course)
                            .Verifiable();

                        enrollmentsMock
                            .Setup(e => e.IsEnrolledAsync(studentId, courseId, ct))
                            .ReturnsAsync(true)
                            .Verifiable();
                    }),
                    ExpectedError = DomainErrors.Enrollment.AlreadyExists
                }
            };

            foreach (var scenario in scenarios)
            {
                // Reset invocations / setups before each scenario to avoid cross-contamination
                coursesMock.Reset();
                enrollmentsMock.Reset();

                uowMock.SetupGet(u => u.Courses).Returns(coursesMock.Object);
                uowMock.SetupGet(u => u.Enrollments).Returns(enrollmentsMock.Object);

                scenario.Setup();

                // Act
                var result = await handler.Handle(cmd, ct);

                // Assert
                Assert.IsNotNull(result, $"{scenario.Name}: result is null");
                Assert.IsTrue(result.IsFailure, $"{scenario.Name}: expected failure");
                Assert.AreEqual(scenario.ExpectedError, result.Error, $"{scenario.Name}: unexpected error value");

                // Verify setups for that scenario
                coursesMock.VerifyAll();
                // Only verify enrollmentsMock when it was setup
                if (scenario.Name == "AlreadyEnrolled")
                    enrollmentsMock.VerifyAll();
            }
        }

        /// <summary>
        /// Tests the successful enrollment flow:
        /// - Course found and published
        /// - Student not already enrolled
        /// - Enrollment is created, added, and saved
        /// Input: Published course, IsEnrolledAsync == false
        /// Expected: Result success with enrollment.Id and repositories SaveChangesAsync/AddAsync called.
        /// </summary>
        [TestMethod]
        public async Task Handle_ValidInput_CreatesEnrollmentAndReturnsId()
        {
            // Arrange
            var studentId = Guid.NewGuid();
            var courseId = Guid.NewGuid();
            var instructorId = Guid.NewGuid();
            var ct = CancellationToken.None;

            var cmd = new EnrollStudentCommand(courseId);

            var uowMock = new Mock<IUnitOfWork>(MockBehavior.Strict);
            var coursesMock = new Mock<ICourseRepository>(MockBehavior.Strict);
            var enrollmentsMock = new Mock<IEnrollmentRepository>(MockBehavior.Strict);

            uowMock.SetupGet(u => u.Courses).Returns(coursesMock.Object);
            uowMock.SetupGet(u => u.Enrollments).Returns(enrollmentsMock.Object);

            // Prepare a published course
            var course = Course.Create(instructorId, Guid.NewGuid(), "Title", "desc", 25.5m, CourseLevel.All, "en");
            course.Publish();

            coursesMock
                .Setup(c => c.GetByIdAsync(courseId, ct))
                .ReturnsAsync(course)
                .Verifiable();

            enrollmentsMock
                .Setup(e => e.IsEnrolledAsync(studentId, courseId, ct))
                .ReturnsAsync(false)
                .Verifiable();

            Enrollment? capturedEnrollment = null;
            enrollmentsMock
                .Setup(e => e.AddAsync(It.IsAny<Enrollment>(), ct))
                .Callback<Enrollment, CancellationToken>((enr, token) => { capturedEnrollment = enr; })
                .Returns(Task.CompletedTask)
                .Verifiable();

            uowMock
                .Setup(u => u.SaveChangesAsync(ct))
                .ReturnsAsync(1)
                .Verifiable();

            var currentMock = new Mock<ICurrentUserService>(MockBehavior.Strict);
            currentMock.SetupGet(c => c.UserId).Returns((Guid?)studentId);

            var handler = new EnrollStudentCommandHandler(uowMock.Object, currentMock.Object);

            // Act
            var result = await handler.Handle(cmd, ct);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsSuccess, "Expected successful result");
            Assert.IsNotNull(capturedEnrollment, "Expected repository AddAsync to be called with an Enrollment");

            // Ensure enrollment fields are set correctly
            Assert.AreEqual(studentId, capturedEnrollment!.StudentId, "Enrollment.StudentId mismatch");
            Assert.AreEqual(courseId, capturedEnrollment.CourseId, "Enrollment.CourseId mismatch");
            Assert.AreEqual(course.Price, capturedEnrollment.PaidPrice, "Enrollment.PaidPrice mismatch");

            // Ensure returned value matches created enrollment id
            Assert.AreEqual(capturedEnrollment.Id, result.Value);

            // Verify interactions
            coursesMock.VerifyAll();
            enrollmentsMock.VerifyAll();
            uowMock.Verify(u => u.SaveChangesAsync(ct), Times.Once);
        }
    }
}