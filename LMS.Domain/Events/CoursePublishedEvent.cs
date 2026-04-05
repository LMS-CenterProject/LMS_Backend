using LMS.Domain.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Domain.Events
{
    public sealed record CoursePublishedEvent(
    Guid CourseId,
    Guid InstructorId) : IDomainEvent;
}
