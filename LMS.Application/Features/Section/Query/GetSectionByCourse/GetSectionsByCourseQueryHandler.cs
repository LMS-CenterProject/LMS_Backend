using LMS.Application.DTOs.Lesson;

using LMS.Application.Features.Section.Query.GetSectionByCourse;

using LMS.Domain.Interfaces.Repositories;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LMS.Application.Features.Section.Queries.GetSectionsByCourse
{
    public class GetSectionsByCourseQueryHandler
        : IRequestHandler<GetSectionsByCourseQuery, List<SectionDto>>
    {
        private readonly ISectionRepository _sectionRepository;

        public GetSectionsByCourseQueryHandler(ISectionRepository sectionRepository)
        {
            _sectionRepository = sectionRepository;
        }

        public async Task<List<SectionDto>> Handle(GetSectionsByCourseQuery request, CancellationToken cancellationToken)
        {
            var sections = await _sectionRepository.GetSectionsWithLessonsByCourseIdAsync(
                request.CourseId,
                cancellationToken);

            return sections.Select(s => new SectionDto
            {
                Id = s.Id,
                Title = s.Title,
                OrderIndex = s.OrderIndex,
                Lessons = s.Lessons
                    .Where(l => !l.IsDeleted)          
                    .OrderBy(l => l.OrderIndex)
                    .Select(l => new LessonDto
                    {
                        Id = l.Id,
                        Title = l.Title,
                        SectionId= l.SectionId,
                        ContentUrl = l.ContentUrl,
                        ContentType = l.ContentType.ToString(),
                        DurationSeconds = l.DurationSeconds,
                        OrderIndex = l.OrderIndex,
                        IsFreePreview = l.IsFreePreview
                    }).ToList()
            }).ToList();
        }
    }
}