using LMS.Application.Common.Interfaces;
using LMS.Domain.Interfaces;
using LMS.Domain.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Quiz.Command.CreateQuiz
{
    public class CreateQuizHandler : IRequestHandler<CreateQuizCommand, Guid>
    {
        private readonly IQuizRepository _quizRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;

        public CreateQuizHandler(
            IQuizRepository quizRepository,
            ICourseRepository courseRepository,
            ICurrentUserService currentUserService,
            IUnitOfWork unitOfWork)
        {
            _quizRepository = quizRepository;
            _courseRepository = courseRepository;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(CreateQuizCommand request, CancellationToken ct)
        {
            var userId = _currentUserService.UserId
                ?? throw new UnauthorizedAccessException();

            var course = await _courseRepository.GetByIdAsync(request.CourseId, ct);

            if (course is null)
                throw new Exception("Course not found");

            if (!course.IsOwner(userId))
                throw new UnauthorizedAccessException();

            var quiz = LMS.Domain.Entities.Quiz.Create(
                request.CourseId,
                request.Title,
                request.PassScore,
                request.TimeLimitMinutes);

            await _quizRepository.AddAsync(quiz, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            return quiz.Id;
        }
    }
}
