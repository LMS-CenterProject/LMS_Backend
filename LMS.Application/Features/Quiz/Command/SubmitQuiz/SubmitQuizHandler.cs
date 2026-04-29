using LMS.Application.Common.Interfaces;
using LMS.Application.DTOs.Quize;
using LMS.Application.Features.Quiz.Command.SubmitQuiz;
using LMS.Domain.Entities;
using LMS.Domain.Interfaces;
using LMS.Domain.Interfaces.Repositories;
using MediatR;

public class SubmitQuizCommandHandler
    : IRequestHandler<SubmitQuizCommand, QuizResultDto>
{
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

        // Check attempts count (optimized)
        var attemptCount = await _attemptRepository
            .CountAttemptsAsync(studentId, request.QuizId, ct);

        if (attemptCount >= 3)
            throw new Exception("Max attempts reached");

        var hasPassed = await _attemptRepository
            .HasPassedAsync(studentId, request.QuizId, ct);

        if (hasPassed)
            throw new Exception("You already passed this quiz");

        if (request.Answers == null || !request.Answers.Any())
            throw new Exception("Answers cannot be empty");

        var (score, passed) = quiz.Grade(request.Answers);

        var attempt = QuizAttempt.Create(
            request.QuizId,
            studentId,
            score,
            passed);

        await _attemptRepository.AddAsync(attempt, ct);

        await _unitOfWork.SaveChangesAsync(ct);

        return new QuizResultDto
        {
            Score = score,
            Passed = passed
        };
    }
}