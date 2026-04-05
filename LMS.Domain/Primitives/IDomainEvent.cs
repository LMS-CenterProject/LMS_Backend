using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace LMS.Domain.Primitives
{
    public interface IDomainEvent : INotification { }
}
