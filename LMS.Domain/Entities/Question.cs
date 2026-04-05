using LMS.Domain.Enums;
using LMS.Domain.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Domain.Entities
{
    public sealed class Question : Entity
    {
        private Question() { }

        public Guid QuizId { get; private set; }
        public string Text { get; private set; } = string.Empty;
        public QuestionType Type { get; private set; }
        public int Points { get; private set; } = 1;

        // Navigation properties
        public Quiz Quiz { get; private set; } = null!;
        public ICollection<Answer> Answers { get; private set; } = [];

        // ── Factory method ───────────────────────────────────────

        public static Question Create(
            Guid quizId,
            string text,
            QuestionType type,
            int points = 1)
        {
            return new Question
            {
                QuizId = quizId,
                Text = text.Trim(),
                Type = type,
                Points = points
            };
        }

        // ── Business methods ─────────────────────────────────────

        public void Update(string text, QuestionType type, int points)
        {
            Text = text.Trim();
            Type = type;
            Points = points;
        }

        public bool HasValidAnswers() =>
            Type == QuestionType.TrueFalse
                ? Answers.Count == 2 && Answers.Count(a => a.IsCorrect) == 1
                : Type == QuestionType.SingleChoice
                    ? Answers.Count(a => a.IsCorrect) == 1
                    : Answers.Any(a => a.IsCorrect);   // MultiChoice: at least one correct
    }
}
