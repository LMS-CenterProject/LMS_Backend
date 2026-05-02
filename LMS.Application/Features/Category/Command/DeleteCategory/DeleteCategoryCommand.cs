using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Category.Command.DeleteCategory
{
    public record DeleteCategoryCommand(Guid Id) : IRequest;
}
