using LMS.Domain.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Domain.Entities
{
    public sealed class RefreshToken : Entity
    {
        private RefreshToken() { }

        public Guid UserId { get; private set; }
        public string Token { get; private set; } = string.Empty;
        public DateTime ExpiresAt { get; private set; }
        public bool IsRevoked { get; private set; }
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public DateTime? RevokedAt { get; private set; }
        public string? ReplacedBy { get; private set; }
        public string? CreatedByIp { get; private set; }

        // Navigation property
        public User User { get; private set; } = null!;

        // ── Computed ─────────────────────────────────────────────

        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
        public bool IsActive => !IsRevoked && !IsExpired;

        // ── Factory method ───────────────────────────────────────

        public static RefreshToken Create(
            Guid userId,
            string token,
            int expiryDays = 7,
            string? createdByIp = null)
        {
            return new RefreshToken
            {
                UserId = userId,
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddDays(expiryDays),
                CreatedByIp = createdByIp
            };
        }

        // ── Business methods ─────────────────────────────────────

        public void Revoke(string? replacedByToken = null)
        {
            IsRevoked = true;
            RevokedAt = DateTime.UtcNow;
            ReplacedBy = replacedByToken;
        }
    }
}
