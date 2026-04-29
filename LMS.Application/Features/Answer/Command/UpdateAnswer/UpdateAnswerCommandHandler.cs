using LMS.Domain.Interfaces;
using LMS.Domain.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Answer.Command.UpdateAnswer
{
    public sealed class UpdateAnswerCommandHandler
        : IRequestHandler<UpdateAnswerCommand, Unit>
    {
        private readonly IAnswerRepository _answerRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateAnswerCommandHandler(
            IAnswerRepository answerRepository,
            IUnitOfWork unitOfWork)
        {
            _answerRepository = answerRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(
            UpdateAnswerCommand request,
            CancellationToken cancellationToken)
        {
            var answer = await _answerRepository
                .GetByIdAsync(request.AnswerId, cancellationToken);

            if (answer is null)
                throw new Exception("Answer not found");

            answer.Update(
                request.Text,
                request.IsCorrect);

            _answerRepository.Update(answer);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
