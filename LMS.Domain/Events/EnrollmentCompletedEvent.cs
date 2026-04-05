using LMS.Domain.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Domain.Events
{
    public sealed record EnrollmentCompletedEvent(
     Guid EnrollmentId,
     Guid StudentId,
     Guid CourseId) : IDomainEvent;
}
