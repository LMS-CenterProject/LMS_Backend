using LMS.Domain.Entities;
using LMS.Domain.Interfaces.Repositories;
using LMS.Infrastructure.Persistence;
using LMS.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

public sealed class QuizRepository
    : Repository<Quiz>, IQuizRepository
{
    private readonly LMSDbContext _context;

    public QuizRepository(LMSDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<Quiz?> GetQuizWithQuestionsAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Quizzes
            .Include(q => q.Questions)
                .ThenInclude(q => q.Answers)
            .FirstOrDefaultAsync(q => q.Id == id, cancellationToken);
    }

    
    public async Task<IEnumerable<Quiz>> GetByCourseIdAsync(
        Guid courseId,
        CancellationToken ct = default)
    {
        return await _context.Quizzes
            .Where(q => q.CourseId == courseId)
            .Include(q => q.Questions)
            .ToListAsync(ct);
    }
}