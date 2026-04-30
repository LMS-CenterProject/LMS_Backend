using LMS.Application.Common.Models;
using LMS.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Admin.Command.ChangeUserRole
{
    public sealed record ChangeUserRoleCommand(Guid UserId, UserRole NewRole) : IRequest<Result>;
    
}
