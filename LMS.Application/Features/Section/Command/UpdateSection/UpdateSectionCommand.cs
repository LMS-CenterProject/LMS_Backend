using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Section.Command.UpdateSection
{
    public sealed record UpdateSectionCommand(
        Guid SectionId,
        string Title,
        int OrderIndex) : IRequest<Unit>;

}
