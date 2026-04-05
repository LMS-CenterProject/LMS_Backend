using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Domain.Primitives
{
    public abstract class AuditableEntity : Entity
    {
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
