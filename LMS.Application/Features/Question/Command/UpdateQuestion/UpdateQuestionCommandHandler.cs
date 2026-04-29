using LMS.Domain.Interfaces;
using LMS.Domain.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Question.Command.UpdateQuestion
{
    public class UpdateQuestionHandler
    : IRequestHandler<UpdateQuestionCommand>
    {
        private readonly IQuestionRepository _questionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateQuestionHandler(
            IQuestionRepository questionRepository,
            IUnitOfWork unitOfWork)
        {
            _questionRepository = questionRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(
            UpdateQuestionCommand request,
            CancellationToken ct)
        {
            var question = await _questionRepository.GetByIdAsync(request.QuestionId, ct);

            if (question is null)
                throw new Exception("Question not found");

            question.Update(
                request.Text,
                request.Type,
                request.Points);

            _questionRepository.Update(question);

            await _unitOfWork.SaveChangesAsync(ct);
        }
    }
}
