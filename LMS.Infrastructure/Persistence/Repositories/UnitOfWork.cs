using LMS.Domain.Interfaces;
using LMS.Domain.Interfaces.Repositories;
using LMS.Domain.Primitives;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Infrastructure.Persistence.Repositories
{
    public sealed class UnitOfWork(LMSDbContext context, IPublisher publisher) : IUnitOfWork
    {
        private IUserRepository? _users;
        private IRefreshTokenRepository? _refreshTokens;
        private IEnrollmentRepository? _enrollments;
        private ICourseRepository? _courses;
        private ILessonProgressRepository? _lessonProgresses;
        private ICertificateRepository? _certificates;
        private ILessonRepository? _lessons;
        private IReviewRepository? _reviews;
        private INotificationRepository? _notifications;     
        private ISectionRepository? _sections;



        public IUserRepository Users => _users ??= new UserRepository(context);
        public IRefreshTokenRepository RefreshTokens => _refreshTokens ??= new RefreshTokenRepository(context);
        public IEnrollmentRepository Enrollments => _enrollments ??= new EnrollmentRepository(context);
        public ICourseRepository Courses => _courses ??= new CourseRepository(context);
        public ILessonProgressRepository LessonProgresses => _lessonProgresses ??= new LessonProgressRepository(context);
        public ICertificateRepository Certificates => _certificates ??= new CertificateRepository(context);
        public ILessonRepository Lessons => _lessons ??= new LessonRepository(context);
        public IReviewRepository Reviews => _reviews ??= new ReviewRepository(context);
        public INotificationRepository Notifications => _notifications ??= new NotificationRepository(context);
        public ISectionRepository Sections => _sections ??= new SectionRepository(context);


        public async Task<int> SaveChangesAsync(CancellationToken ct = default)
        {
            // 1. Save changes first
            var result = await context.SaveChangesAsync(ct);

            // 2. Collect and dispatch domain events raised by entities
            var domainEvents = context.ChangeTracker
                .Entries<Entity>()
                .Select(e => e.Entity)
                .Where(e => e.DomainEvents.Any())
                .SelectMany(e =>
                {
                    var events = e.DomainEvents.ToList();
                    e.ClearDomainEvents();
                    return events;
                })
                .ToList();

            foreach (var domainEvent in domainEvents)
                await publisher.Publish(domainEvent, ct);

            return result;
        }
        public void Dispose() => context.Dispose();
    }
}
