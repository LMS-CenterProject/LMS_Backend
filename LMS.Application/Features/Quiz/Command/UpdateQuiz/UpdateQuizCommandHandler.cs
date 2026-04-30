using LMS.Application.Common.Interfaces;
using LMS.Domain.Interfaces;
using LMS.Domain.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Quiz.Command.UpdateQuiz
{
    public class UpdateQuizHandler : IRequestHandler<UpdateQuizCommand>
    {
        private readonly IQuizRepository _quizRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICourseRepository _courseRepository;
        private readonly ICurrentUserService _currentUserService;
        public UpdateQuizHandler(IQuizRepository quizRepository, IUnitOfWork unitOfWork, ICourseRepository courseRepository, ICurrentUserService currentUserService)
        {
            _quizRepository = quizRepository;
            _unitOfWork = unitOfWork;
            _courseRepository = courseRepository;
            _currentUserService = currentUserService;
        }
        public async Task Handle(UpdateQuizCommand request, CancellationToken ct)
        {

            var quiz = await _quizRepository.GetByIdAsync(request.QuizId, ct);

            if (quiz is null)
                throw new Exception("Quiz not found");

            quiz.Update(
                request.Title,
                request.PassScore,
                request.TimeLimitMinutes);

            _quizRepository.Update(quiz);

            await _unitOfWork.SaveChangesAsync(ct);
        }
    }
}
