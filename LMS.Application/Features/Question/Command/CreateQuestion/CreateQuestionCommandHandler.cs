using LMS.Domain.Interfaces;
using LMS.Domain.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Question.Command.CreateQuestion
{
    public class CreateQuestionHandler
    : IRequestHandler<CreateQuestionCommand, Guid>
    {
        private readonly IQuestionRepository _questionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateQuestionHandler(
            IQuestionRepository questionRepository,
            IUnitOfWork unitOfWork)
        {
            _questionRepository = questionRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(
            CreateQuestionCommand request,
            CancellationToken ct)
        {
            var question = LMS.Domain.Entities.Question.Create(
                request.QuizId,
                request.Text,
                request.Type,
                request.Points);

            await _questionRepository.AddAsync(question, ct);

            await _unitOfWork.SaveChangesAsync(ct);

            return question.Id;
        }
    }
}
