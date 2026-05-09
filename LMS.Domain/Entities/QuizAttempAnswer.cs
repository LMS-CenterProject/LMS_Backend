using LMS.Domain.Primitives;

namespace LMS.Domain.Entities
{
    public sealed class QuizAttemptAnswer : Entity
    {
        private QuizAttemptAnswer() { }

        public Guid QuizAttemptId { get; private set; }
        public Guid QuestionId { get; private set; }
        public Guid AnswerId { get; private set; }

        // Navigation properties
        public QuizAttempt Attempt { get; private set; } = null!;
        public Question Question { get; private set; } = null!;
        public Answer Answer { get; private set; } = null!;

        public static QuizAttemptAnswer Create(Guid attemptId, Guid questionId, Guid answerId) =>
            new()
            {
                QuizAttemptId = attemptId,
                QuestionId = questionId,
                AnswerId = answerId
            };
    }
}
