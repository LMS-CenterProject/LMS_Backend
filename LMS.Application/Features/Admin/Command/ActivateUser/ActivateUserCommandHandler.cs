using LMS.Application.Common.Models;
using LMS.Domain.Errors;
using LMS.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Admin.Command.ActivateUser
{
    public sealed class ActivateUserCommandHandler(IUnitOfWork uow)
        : IRequestHandler<ActivateUserCommand, Result>
    {
        public async Task<Result> Handle(
            ActivateUserCommand cmd, CancellationToken ct)
        {
            var user = await uow.Users.GetByIdAsync(cmd.UserId, ct);

            if (user is null)
                return Result.Failure(DomainErrors.User.NotFound);

            user.Activate();
            await uow.SaveChangesAsync(ct);
            return Result.Success();
        }
    }
}
