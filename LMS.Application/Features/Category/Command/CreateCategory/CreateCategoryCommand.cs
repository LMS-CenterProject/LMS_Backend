using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Category.Command.CreateCategory
{
    public record CreateCategoryCommand(string Name) : IRequest<Guid>;
}
