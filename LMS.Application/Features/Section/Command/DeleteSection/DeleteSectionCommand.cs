using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace LMS.Application.Features.Section.Command.DeleteSection
{
    public sealed record DeleteSectionCommand(Guid SectionId) : IRequest<Unit>;

}
