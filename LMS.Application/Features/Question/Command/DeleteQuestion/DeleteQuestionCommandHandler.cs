using LMS.Domain.Interfaces;
using LMS.Domain.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Question.Command.DeleteQuestion
{
    public class DeleteQuestionHandler
    : IRequestHandler<DeleteQuestionCommand>
    {
        private readonly IQuestionRepository _questionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteQuestionHandler(
            IQuestionRepository questionRepository,
            IUnitOfWork unitOfWork)
        {
            _questionRepository = questionRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(
            DeleteQuestionCommand request,
            CancellationToken ct)
        {
            var question = await _questionRepository.GetByIdAsync(request.QuestionId, ct);

            if (question is null)
                throw new Exception("Question not found");

            _questionRepository.Remove(question);

            await _unitOfWork.SaveChangesAsync(ct);
        }
    }
}
