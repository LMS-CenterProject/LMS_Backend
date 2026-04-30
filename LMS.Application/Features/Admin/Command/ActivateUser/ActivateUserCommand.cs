using LMS.Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Admin.Command.ActivateUser
{
    public sealed record ActivateUserCommand(Guid UserId) : IRequest<Result>;
    
    
}
