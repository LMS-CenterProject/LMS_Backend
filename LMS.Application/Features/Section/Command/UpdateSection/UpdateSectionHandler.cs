using LMS.Application.Common.Interfaces;
using LMS.Domain.Interfaces;
using LMS.Domain.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Section.Command.UpdateSection
{
    public sealed class UpdateSectionHandler : IRequestHandler<UpdateSectionCommand, Unit>
    {
        private readonly ICourseRepository _courseRepository;
        private readonly ISectionRepository _sectionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public UpdateSectionHandler(ISectionRepository sectionRepository, ICourseRepository courseRepository, IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _sectionRepository = sectionRepository;
            _courseRepository = courseRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;

        }
        public async Task<Unit> Handle(UpdateSectionCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;

            var section = await _sectionRepository.GetByIdAsync(request.SectionId, cancellationToken);

            if (section is null)
                throw new Exception("Section not found");

            var course = await _courseRepository.GetByIdAsync(section.CourseId, cancellationToken);

            if (!course.IsOwner(userId.Value))
                throw new UnauthorizedAccessException();

            section.Update(request.Title, request.OrderIndex);

            _sectionRepository.Update(section);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;

        }
    }
}
