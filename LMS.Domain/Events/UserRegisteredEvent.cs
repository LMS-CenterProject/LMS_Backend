using LMS.Domain.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Domain.Events
{
    public sealed record UserRegisteredEvent(
    Guid UserId,
    string Email,
    string FullName) : IDomainEvent;
}
