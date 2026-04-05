using LMS.Domain.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Domain.Entities
{
    public sealed class QuizAttempt : Entity
    {
        private QuizAttempt() { }

        public Guid QuizId { get; private set; }
        public Guid StudentId { get; private set; }
        public int Score { get; private set; }
        public bool Passed { get; private set; }
        public DateTime AttemptedAt { get; private set; } = DateTime.UtcNow;

        // Navigation properties
        public Quiz Quiz { get; private set; } = null!;
        public User Student { get; private set; } = null!;

        // ── Factory method ───────────────────────────────────────

        public static QuizAttempt Create(Guid quizId, Guid studentId, int score, bool passed) =>
            new()
            {
                QuizId = quizId,
                StudentId = studentId,
                Score = score,
                Passed = passed
            };
    }
}
