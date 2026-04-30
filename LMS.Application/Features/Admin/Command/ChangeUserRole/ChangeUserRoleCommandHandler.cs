using LMS.Application.Common.Interfaces;
using LMS.Application.Common.Models;
using LMS.Domain.Enums;
using LMS.Domain.Errors;
using LMS.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Admin.Command.ChangeUserRole
{
    public sealed class ChangeUserRoleCommandHandler(IUnitOfWork uow, ICurrentUserService current) : IRequestHandler<ChangeUserRoleCommand, Result>
    {
        public async Task<Result> Handle(
            ChangeUserRoleCommand cmd, CancellationToken ct)
        {
            var user = await uow.Users.GetByIdAsync(cmd.UserId, ct);

            if (user is null)
                return Result.Failure(DomainErrors.User.NotFound);

            // Cannot change your own role
            if (cmd.UserId == current.UserId)
                return Result.Failure(DomainErrors.User.CannotChangeOwnRole);

            // Only SuperAdmin can assign SuperAdmin role
            if (cmd.NewRole == UserRole.SuperAdmin)
            {
                var requester = await uow.Users.GetByIdAsync(current.UserId!.Value, ct);
                if (requester?.Role != UserRole.SuperAdmin)
                    return Result.Failure(DomainErrors.User.UnAuthorized);
            }

            user.ChangeRole(cmd.NewRole);
            await uow.SaveChangesAsync(ct);
            return Result.Success();
        }
    }
}
