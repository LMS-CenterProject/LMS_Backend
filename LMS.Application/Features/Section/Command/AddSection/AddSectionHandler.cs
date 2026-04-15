using LMS.Application.Common.Interfaces;
using LMS.Domain.Entities;                 
using LMS.Domain.Interfaces;
using LMS.Domain.Interfaces.Repositories;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LMS.Application.Features.Section.Command.CreateSection
{
    public sealed class AddSectionHandler
        : IRequestHandler<AddSectionCommand, Guid>
    {
        private readonly ICourseRepository _courseRepository;
        private readonly ISectionRepository _sectionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public AddSectionHandler(
            ICourseRepository courseRepository,
            ISectionRepository sectionRepository,
            IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _courseRepository = courseRepository;
            _sectionRepository = sectionRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Guid> Handle(AddSectionCommand request, CancellationToken cancellationToken)
        {

            var userId = _currentUserService.UserId;

            if (userId is null)
                throw new UnauthorizedAccessException();

            var course = await _courseRepository.GetByIdAsync(request.CourseId, cancellationToken);

            if (course is null)
                throw new Exception("Course not found");

            // 🔥 Ownership check
            if (!course.IsOwner(userId.Value))
                throw new UnauthorizedAccessException("Not your course");


            var section = LMS.Domain.Entities.Section.Create(
                request.CourseId,
                request.Title,
                request.OrderIndex);

            
            await _sectionRepository.AddAsync(section, cancellationToken);

            
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return section.Id;
        }
    }
}