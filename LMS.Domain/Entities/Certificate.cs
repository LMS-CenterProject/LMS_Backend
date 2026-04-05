using LMS.Domain.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Domain.Entities
{
    public sealed class Certificate : Entity
    {
        private Certificate() { }

        public Guid EnrollmentId { get; private set; }
        public string CertificateUrl { get; private set; } = string.Empty;
        public DateTime IssuedAt { get; private set; } = DateTime.UtcNow;

        // Navigation property
        public Enrollment Enrollment { get; private set; } = null!;

        // ── Factory method ───────────────────────────────────────

        public static Certificate Issue(Guid enrollmentId, string certificateUrl) =>
            new() { EnrollmentId = enrollmentId, CertificateUrl = certificateUrl };
    }
}
