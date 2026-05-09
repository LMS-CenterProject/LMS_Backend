using LMS.Domain.Primitives;
using System;
using System.Collections.Generic;
using System.Text;
using static LMS.Domain.Errors.DomainErrors;

namespace LMS.Domain.Entities
{
    public sealed class QuizAttempt : Entity
    {
        private readonly List<QuizAttemptAnswer> _answers = [];
        private QuizAttempt() { }

        public Guid QuizId { get; private set; }
        public Guid StudentId { get; private set; }
        public int Score { get; private set; }
        public bool Passed { get; private set; }
        public DateTime AttemptedAt { get; private set; } = DateTime.UtcNow;

        // Navigation properties
        public Quiz Quiz { get; private set; } = null!;
        public User Student { get; private set; } = null!;
        public IReadOnlyCollection<QuizAttemptAnswer> Answers => _answers.AsReadOnly();

        // ── Factory method ───────────────────────────────────────

        public static QuizAttempt Create(
           Guid quizId,
           Guid studentId,
           int score,
           bool passed,
           Dictionary<Guid, List<Guid>> selectedAnswers)
        {
            var attempt = new QuizAttempt
            {
                QuizId = quizId,
                StudentId = studentId,
                Score = score,
                Passed = passed
            };

            foreach (var (questionId, answerIds) in selectedAnswers)
                foreach (var answerId in answerIds)
                    attempt._answers.Add(
                        QuizAttemptAnswer.Create(attempt.Id, questionId, answerId));

            return attempt;
        }
    }
}
