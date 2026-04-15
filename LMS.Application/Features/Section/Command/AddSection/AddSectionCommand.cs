using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Section.Command.CreateSection
{
    public sealed record AddSectionCommand(
         Guid CourseId,
        string Title,
        int OrderIndex) : IRequest<Guid>;

}
