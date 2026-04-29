using LMS.Domain.Interfaces;
using LMS.Domain.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Quiz.Command.DeleteQuiz
{
    public class DeleteQuizHandler : IRequestHandler<DeleteQuizCommand>
    {
        private readonly IQuizRepository _quizRepository;
        private readonly IUnitOfWork _unitOfWork;
        public DeleteQuizHandler(IQuizRepository quizRepository, IUnitOfWork unitOfWork)
        {
            _quizRepository = quizRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task Handle(DeleteQuizCommand request, CancellationToken ct)
        {
            var quiz = await _quizRepository.GetByIdAsync(request.QuizId, ct);

            if (quiz is null)
                throw new Exception("Quiz not found");

            _quizRepository.Remove(quiz);

            await _unitOfWork.SaveChangesAsync(ct);
        }
    }
}
