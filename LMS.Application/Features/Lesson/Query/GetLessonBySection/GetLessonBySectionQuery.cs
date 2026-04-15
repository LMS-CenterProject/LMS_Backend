using LMS.Application.DTOs.Lesson;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Lesson.Query.GetLessonBySection
{
    public sealed record GetLessonsBySectionQuery(Guid SectionId)
        : IRequest<List<LessonDto>>;
}
