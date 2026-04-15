using LMS.Application.Common.Interfaces;
using LMS.Domain.Interfaces;
using LMS.Domain.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Section.Command.DeleteSection
{
    public sealed class DeleteSectionHandler : IRequestHandler<DeleteSectionCommand,Unit>
    {
        private readonly ICourseRepository _courseRepository;
        private readonly ISectionRepository _sectionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public DeleteSectionHandler(ISectionRepository sectionRepository, ICourseRepository courseRepository, IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _sectionRepository = sectionRepository;
            _courseRepository = courseRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }
        public async Task<Unit> Handle(DeleteSectionCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;

            var section = await _sectionRepository.GetByIdAsync(request.SectionId);

            var course = await _courseRepository.GetByIdAsync(section.CourseId);

            if (!course.IsOwner(userId.Value))
                throw new UnauthorizedAccessException();

            section.SoftDelete();
            _sectionRepository.Update(section);
            _unitOfWork.SaveChangesAsync(cancellationToken);
            return Unit.Value;

        }
    }
}
