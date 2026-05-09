using FluentValidation;
using LMS.Application.Features.Quiz.Command.SubmitQuiz;

namespace LMS.Application.Features.Quiz.Command.SubmitQuiz
{
    public class SubmitQuizCommandValidator : AbstractValidator<SubmitQuizCommand>
    {
        public SubmitQuizCommandValidator()
        {
            RuleFor(x => x.QuizId)
                .NotEmpty()
                .WithMessage("QuizId is required.");

            RuleFor(x => x.Answers)
                .NotNull().WithMessage("Answers cannot be null.")
                .NotEmpty().WithMessage("You must answer at least one question.");

            RuleForEach(x => x.Answers).ChildRules(answer =>
            {
                answer.RuleFor(a => a.QuestionId)
                    .NotEmpty()
                    .WithMessage("Each answer must have a valid QuestionId.");

                answer.RuleFor(a => a.AnswerId)
                    .NotEmpty()
                    .WithMessage("AnswerId cannot be empty.");
            });

            // Prevent duplicate question entries in the submission
            RuleFor(x => x.Answers)
                .Must(answers => 
                {
                    if (answers == null || answers.Count == 0)
                        return true;

                    var distinctQuestions = answers.Select(a => a.QuestionId).Distinct().Count();
                    return distinctQuestions == answers.Count;
                })
                .WithMessage("Each question can only be answered once.");
        }
    }
}