using LMS.Domain.Interfaces;
using LMS.Domain.Interfaces.Repositories;
using MediatR;
namespace LMS.Application.Features.Question.Command.CreateQuestion
{
    public class CreateQuestionCommandHandler
    : IRequestHandler<CreateQuestionCommand, Guid>
    {
        private readonly IQuestionRepository _questionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateQuestionCommandHandler(
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
            // 1. create question
            var question = LMS.Domain.Entities.Question.Create(
                request.QuizId,
                request.Text,
                request.Type,
                request.Points
            );

            

            //if (!question.HasValidAnswers())
            //    throw new Exception("Invalid answers for this question type");

            // 4. save
            await _questionRepository.AddAsync(question);
            await _unitOfWork.SaveChangesAsync(ct);

            return question.Id;
        }
    }
}
