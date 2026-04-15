using LMS.Domain.Interfaces;
using LMS.Domain.Interfaces.Repositories;
using MediatR;

namespace LMS.Application.Features.Courses.Commands.ArchiveCourse
{
    public class ArchiveCourseHandler:IRequestHandler<ArchiveCourseCommand>
    {
        private readonly ICourseRepository _courseRepo;
        private readonly IUnitOfWork _unitOfWork;
        public ArchiveCourseHandler(ICourseRepository courseRepo, IUnitOfWork unitOfWork)
        {
            _courseRepo = courseRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(ArchiveCourseCommand request, CancellationToken cancellationToken)
        {
            var course = await _courseRepo.GetByIdAsync(request.CourseId, cancellationToken);
            if (course == null)
            {
                throw new Exception($"Course not found.");
            }
            course.Archive();
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

    }
}
