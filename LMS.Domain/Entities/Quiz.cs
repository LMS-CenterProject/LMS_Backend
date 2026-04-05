using LMS.Domain.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Domain.Entities
{
    public sealed class Quiz : Entity
    {
        private Quiz() { }

        public Guid CourseId { get; private set; }
        public string Title { get; private set; } = string.Empty;
        public int PassScore { get; private set; } = 70;
        public int? TimeLimitMinutes { get; private set; }

        // Navigation properties
        public Course Course { get; private set; } = null!;
        public ICollection<Question> Questions { get; private set; } = [];
        public ICollection<QuizAttempt> Attempts { get; private set; } = [];

        // ── Factory method ───────────────────────────────────────

        public static Quiz Create(
            Guid courseId,
            string title,
            int passScore = 70,
            int? timeLimitMinutes = null)
        {
            return new Quiz
            {
                CourseId = courseId,
                Title = title.Trim(),
                PassScore = passScore,
                TimeLimitMinutes = timeLimitMinutes
            };
        }

        // ── Business methods ─────────────────────────────────────

        public void Update(string title, int passScore, int? timeLimitMinutes)
        {
            Title = title.Trim();
            PassScore = passScore;
            TimeLimitMinutes = timeLimitMinutes;
        }

        /// <summary>
        /// Grades a submission. Keys are QuestionIds, values are selected AnswerIds.
        /// Returns score as a percentage (0–100).
        /// </summary>
        public (int Score, bool Passed) Grade(Dictionary<Guid, List<Guid>> answers)
        {
            if (Questions.Count == 0) return (0, false);

            int totalPoints = Questions.Sum(q => q.Points);
            int earnedPoints = 0;

            foreach (var question in Questions)
            {
                if (!answers.TryGetValue(question.Id, out var selected)) continue;

                var correctIds = question.Answers.Where(a => a.IsCorrect).Select(a => a.Id).ToHashSet();
                var selectedIds = selected.ToHashSet();

                // All-or-nothing grading: full points only when selection exactly matches correct answers
                if (correctIds.SetEquals(selectedIds))
                    earnedPoints += question.Points;
            }

            int score = totalPoints == 0 ? 0 : (int)Math.Round((double)earnedPoints / totalPoints * 100);
            bool passed = score >= PassScore;

            return (score, passed);
        }
    }
}
