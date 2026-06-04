using LMS.Application.Common.Interfaces;
using LMS.Application.DTOs.Quiz;
using LMS.Application.Features.Quiz.Command.SubmitQuiz;
using LMS.Domain.Entities;
using LMS.Domain.Interfaces;
using LMS.Domain.Interfaces.Repositories;
using MediatR;
using System.Linq.Expressions;

public class SubmitQuizCommandHandler
    : IRequestHandler<SubmitQuizCommand, QuizResultDto>
{
    private const int MaxAttempts = 3;
    private readonly IQuizRepository _quizRepository;
    private readonly IQuizAttemptRepository _attemptRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SubmitQuizCommandHandler(
        IQuizRepository quizRepository,
        IQuizAttemptRepository attemptRepository,
        ICurrentUserService currentUser,
        IEnrollmentRepository enrollmentRepository,
        IUnitOfWork unitOfWork)
    {
        _quizRepository = quizRepository;
        _attemptRepository = attemptRepository;
        _currentUser = currentUser;
        _enrollmentRepository = enrollmentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<QuizResultDto> Handle(
        SubmitQuizCommand request,
        CancellationToken ct)
    {
        if (!_currentUser.UserId.HasValue)
            throw new UnauthorizedAccessException();

        var studentId = _currentUser.UserId.Value;

        var quiz = await _quizRepository
            .GetQuizWithQuestionsAsync(request.QuizId, ct);

        if (quiz is null)
            throw new Exception("Quiz not found");

        // Check enrollment 
        var isEnrolled = await _enrollmentRepository
            .IsEnrolledAsync(studentId, quiz.CourseId, ct);

        if (!isEnrolled)
            throw new Exception("You are not enrolled in this course");

        // Check attempts count
        var previousAttempts = await _attemptRepository
                .GetByStudentAndQuizAsync(studentId, request.QuizId, ct);

        if (previousAttempts.Count >= MaxAttempts)
            throw new Exception(
                $"You have reached the maximum of {MaxAttempts} attempts for this quiz.");

        if (previousAttempts.Any(a => a.Passed))
            throw new Exception("You have already passed this quiz.");

        // Validate all questions were answered
        var submittedQuestionIds = request.Answers
                .Select(a => a.QuestionId)
                .ToHashSet();

        var quizQuestionIds = quiz.Questions
            .Select(q => q.Id)
            .ToHashSet();

        if (!quizQuestionIds.SetEquals(submittedQuestionIds))
        {
            var missing = quizQuestionIds.Except(submittedQuestionIds);
            throw new Exception(
                $"Missing answers for {missing.Count()} question(s). All questions must be answered.");
        }

        // Grade the quiz
        // Correctly group multiple answers for the same question (e.g., multi-choice questions)
        var answersDictionary = request.Answers
            .GroupBy(a => a.QuestionId)
            .ToDictionary(
                g => g.Key,
                g => g.Select(x => x.AnswerId).ToList());

        var (score, passed) = quiz.Grade(answersDictionary);

        var attempt = QuizAttempt.Create(
                request.QuizId,
                studentId,
                score,
                passed,
                answersDictionary);       // ← new parameter

        await _attemptRepository.AddAsync(attempt, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        // Return richer DTO ──────────────────────────────────────────────
        return new QuizResultDto
        {
            AttemptId = attempt.Id,
            Score = score,
            Passed = passed,
            RemainingAttempts = MaxAttempts - (previousAttempts.Count + 1)
        };
    }
}