using LMS.Domain.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Domain.Entities
{
    public sealed class Notification : Entity
    {
        private Notification() { }

        public Guid UserId { get; private set; }
        public string Title { get; private set; } = string.Empty;
        public string Message { get; private set; } = string.Empty;
        public bool IsRead { get; private set; }
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

        // Navigation property
        public User User { get; private set; } = null!;

        // ── Factory method ───────────────────────────────────────

        public static Notification Create(Guid userId, string title, string message) =>
            new()
            {
                UserId = userId,
                Title = title.Trim(),
                Message = message.Trim()
            };

        // ── Business methods ─────────────────────────────────────

        public void MarkAsRead() => IsRead = true;
        public void MarkAsUnread() => IsRead = false;
    }
}
