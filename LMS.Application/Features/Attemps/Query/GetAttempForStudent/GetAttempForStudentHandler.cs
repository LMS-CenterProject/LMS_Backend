using LMS.Application.DTOs.Quiz;
using LMS.Domain.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LMS.Application.DTOs.Quiz;

namespace LMS.Application.Features.Attemps.Query.GetAttempForStudent
{
    public class GetAttempForStudentHandler:IRequestHandler<GetAttempForStudentQuery, QuizStatsDto>
    {
        private IQuizAttemptRepository _quizAttemp;
        public GetAttempForStudentHandler(IQuizAttemptRepository quizAttemp)
        {
            _quizAttemp = quizAttemp;
        }
        public async Task<QuizStatsDto> Handle(GetAttempForStudentQuery request, CancellationToken cancellationToken)
        {
            var attemp = await _quizAttemp.GetByQuizIdAsync(request.QuizId);
            return new QuizStatsDto
            {
                QuizId = request.QuizId,
                TotalAttempts = attemp.Count(),
                PassedCount = attemp.Count(a => a.Passed),
                FailedCount = attemp.Count(a => !a.Passed),
                AverageScore = attemp.Any() ? (int)attemp.Average(a => a.Score) : 0
            };
        }
    }
}
