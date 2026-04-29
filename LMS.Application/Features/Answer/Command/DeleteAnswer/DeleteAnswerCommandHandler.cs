using LMS.Domain.Interfaces;
using LMS.Domain.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Answer.Command.DeleteAnswer
{
    public sealed class DeleteAnswerCommandHandler
       : IRequestHandler<DeleteAnswerCommand, Unit>
    {
        private readonly IAnswerRepository _answerRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteAnswerCommandHandler(
            IAnswerRepository answerRepository,
            IUnitOfWork unitOfWork)
        {
            _answerRepository = answerRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(
            DeleteAnswerCommand request,
            CancellationToken cancellationToken)
        {
            var answer = await _answerRepository
                .GetByIdAsync(request.AnswerId, cancellationToken);

            if (answer is null)
                throw new Exception("Answer not found");

            _answerRepository.Remove(answer);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
