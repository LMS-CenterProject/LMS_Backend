using LMS.Application.Common.Models;
using LMS.Domain.Errors;
using LMS.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Admin.Command.DeactivateUser
{
    public sealed class DeactivateUserCommandHandler(IUnitOfWork uow): IRequestHandler<DeactivateUserCommand, Result>
    {
        public async Task<Result> Handle(
            DeactivateUserCommand cmd, CancellationToken ct)
        {
            var user = await uow.Users.GetByIdAsync(cmd.UserId, ct);
            if (user is null)
                return Result.Failure(DomainErrors.User.NotFound);
            user.Deactivate();
            await uow.SaveChangesAsync(ct);
            return Result.Success();
        }
    }
}
