using LMS.Application.DTOs.Lesson;
using LMS.Domain.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Lesson.Query.GetLessonBySection
{
    public class GetLessonsBySectionQueryHandler
        : IRequestHandler<GetLessonsBySectionQuery, List<LessonDto>>
    {
        private readonly ILessonRepository _lessonRepository;

        public GetLessonsBySectionQueryHandler(ILessonRepository lessonRepository)
        {
            _lessonRepository = lessonRepository;
        }

        public async Task<List<LessonDto>> Handle(GetLessonsBySectionQuery request, CancellationToken cancellationToken)
        {
            var lessons = await _lessonRepository.GetLessonsBySectionIdAsync(request.SectionId, cancellationToken);

            return lessons.Select(l => new LessonDto
            {
                Id = l.Id,
                SectionId = l.SectionId,
                Title = l.Title,
                ContentUrl = l.ContentUrl,
                ContentType = l.ContentType.ToString(),
                DurationSeconds = l.DurationSeconds,
                OrderIndex = l.OrderIndex,
                IsFreePreview = l.IsFreePreview
            }).ToList();
        }
    }
}
