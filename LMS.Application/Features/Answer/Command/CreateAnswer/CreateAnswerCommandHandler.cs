using LMS.Domain.Interfaces;
using LMS.Domain.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Answer.Command.CreateAnswer
{
    public sealed class CreateAnswerCommandHandler
        : IRequestHandler<CreateAnswerCommand, Guid>
    {
        private readonly IAnswerRepository _answerRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateAnswerCommandHandler(
            IAnswerRepository answerRepository,
            IUnitOfWork unitOfWork)
        {
            _answerRepository = answerRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(
            CreateAnswerCommand request,
            CancellationToken cancellationToken)
        {
            var answer = LMS.Domain.Entities.Answer.Create(
                request.QuestionId,
                request.Text,
                request.IsCorrect);

            await _answerRepository.AddAsync(answer, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return answer.Id;
        }
    }
}
