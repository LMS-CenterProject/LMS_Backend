
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Section.Query.GetSectionByCourse
{
    public sealed record GetSectionsByCourseQuery(Guid CourseId)
        : IRequest<List<SectionDto>>;
}
