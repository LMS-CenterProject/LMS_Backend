using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Category.Command.UpdateCategory
{
    public record UpdateCategoryCommand(Guid Id, string Name) : IRequest;
}
