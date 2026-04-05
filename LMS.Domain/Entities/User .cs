using LMS.Domain.Enums;
using LMS.Domain.Events;
using LMS.Domain.Primitives;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using static LMS.Domain.Errors.DomainErrors;

namespace LMS.Domain.Entities
{
    public sealed class User : AuditableEntity
    {
        private User() { }

        public string FullName { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;
        public string? PhoneNumber { get; private set; }
        public string? PasswordHash { get; private set; }
        public string? GoogleId { get; private set; }
        public AuthProvider AuthProvider { get; private set; } = AuthProvider.Local;
        public string? AvatarUrl { get; private set; }
        public UserRole Role { get; private set; } = UserRole.Student;
        public bool IsActive { get; private set; } = true;

        // Navigation properties
        public ICollection<Course> Courses { get; private set; } = [];
        public ICollection<Enrollment> Enrollments { get; private set; } = [];
        public ICollection<RefreshToken> RefreshTokens { get; private set; } = [];
        public ICollection<Notification> Notifications { get; private set; } = [];
        public ICollection<Review> Reviews { get; private set; } = [];

        // ── Factory methods ──────────────────────────────────────

        public static User CreateLocal(
            string fullName,
            string email,
            string passwordHash,
            string? phoneNumber = null)
        {
            var user = new User
            {
                FullName = fullName.Trim(),
                Email = email.Trim().ToLowerInvariant(),
                PasswordHash = passwordHash,
                PhoneNumber = phoneNumber?.Trim(),
                AuthProvider = AuthProvider.Local,
                Role = UserRole.Student
            };

            user.RaiseDomainEvent(new UserRegisteredEvent(user.Id, user.Email, user.FullName));
            return user;
        }

        public static User CreateWithGoogle(
            string fullName,
            string email,
            string googleId,
            string? avatarUrl = null)
        {
            var user = new User
            {
                FullName = fullName.Trim(),
                Email = email.Trim().ToLowerInvariant(),
                GoogleId = googleId,
                AvatarUrl = avatarUrl,
                AuthProvider = AuthProvider.Google,
                Role = UserRole.Student
            };

            user.RaiseDomainEvent(new UserRegisteredEvent(user.Id, user.Email, user.FullName));
            return user;
        }

        // ── Business methods ─────────────────────────────────────

        public void UpdateProfile(string fullName, string? phoneNumber, string? avatarUrl)
        {
            FullName = fullName.Trim();
            PhoneNumber = phoneNumber?.Trim();
            AvatarUrl = avatarUrl;
            UpdatedAt = DateTime.UtcNow;
        }

        public void LinkGoogle(string googleId)
        {
            GoogleId = googleId;
            UpdatedAt = DateTime.UtcNow;
        }

        public void ChangeRole(UserRole role)
        {
            Role = role;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Activate()
        {
            IsActive = true;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Deactivate()
        {
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdatePassword(string newPasswordHash)
        {
            PasswordHash = newPasswordHash;
            UpdatedAt = DateTime.UtcNow;
        }

        // ── Computed ─────────────────────────────────────────────

        public bool IsInstructor => Role is UserRole.Instructor or UserRole.Admin or UserRole.SuperAdmin;
        public bool IsAdmin => Role is UserRole.Admin or UserRole.SuperAdmin;
    }
}
