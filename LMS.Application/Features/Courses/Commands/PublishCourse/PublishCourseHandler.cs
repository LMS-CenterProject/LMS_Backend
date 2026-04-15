using System;
using System.Collections.Generic;
using System.Text;
using LMS.Domain.Interfaces;
using LMS.Domain.Interfaces.Repositories;
using MediatR;

namespace LMS.Application.Features.Courses.Commands.PublishCourse
{
    public sealed class PublishCourseHandler:IRequestHandler<PublishCourseCommand>
    {
        private readonly ICourseRepository _courseRepo;
        private readonly IUnitOfWork _unitOfWork;
        public PublishCourseHandler(ICourseRepository courseRepo, IUnitOfWork unitOfWork)
        {
            _courseRepo = courseRepo;
            _unitOfWork = unitOfWork;
        }
        public async Task Handle(
            PublishCourseCommand request,
            CancellationToken cancellationToken)
        {
            var course = await _courseRepo
                .GetByIdAsync(request.CourseId, cancellationToken);

            if (course is null)
                throw new Exception("Course not found");

            course.Publish();

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
