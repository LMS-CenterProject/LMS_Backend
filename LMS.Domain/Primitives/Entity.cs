using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Domain.Primitives
{
    public abstract class Entity
    {
        private readonly List<IDomainEvent> _domainEvents = [];

        protected Entity() { }

        public Guid Id { get; init; } = Guid.NewGuid();

        public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        protected void RaiseDomainEvent(IDomainEvent domainEvent) =>
            _domainEvents.Add(domainEvent);

        public void ClearDomainEvents() => _domainEvents.Clear();
    }
}
