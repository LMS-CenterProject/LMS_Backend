using LMS.Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Admin.Command.DeactivateUser
{
    public sealed record DeactivateUserCommand(Guid UserId) : IRequest<Result>;
}
