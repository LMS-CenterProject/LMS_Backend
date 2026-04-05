using LMS.Domain.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Domain.Entities
{
    public sealed class Answer : Entity
    {
        private Answer() { }

        public Guid QuestionId { get; private set; }
        public string Text { get; private set; } = string.Empty;
        public bool IsCorrect { get; private set; }

        // Navigation property
        public Question Question { get; private set; } = null!;

        // ── Factory method ───────────────────────────────────────

        public static Answer Create(Guid questionId, string text, bool isCorrect) =>
            new() { QuestionId = questionId, Text = text.Trim(), IsCorrect = isCorrect };

        // ── Business methods ─────────────────────────────────────

        public void Update(string text, bool isCorrect)
        {
            Text = text.Trim();
            IsCorrect = isCorrect;
        }
    }
}
