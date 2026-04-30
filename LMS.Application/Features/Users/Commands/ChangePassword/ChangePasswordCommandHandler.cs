using LMS.Application.Common.Interfaces;
using LMS.Application.Common.Models;
using LMS.Domain.Enums;
using LMS.Domain.Errors;
using LMS.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Users.Commands.ChangePassword
{
    public sealed class ChangePasswordCommandHandler(
        IUnitOfWork uow,IPasswordHasher hasher,ICurrentUserService current) : IRequestHandler<ChangePasswordCommand, Result>
    {
        public async Task<Result> Handle(ChangePasswordCommand cmd, CancellationToken ct)
        {
            var user = await uow.Users.GetByIdAsync(current.UserId.Value, ct);
            if (user == null)
            {
                return Result.Failure(DomainErrors.User.NotFound);
            }
            //OAuth

            if (user.AuthProvider == AuthProvider.Google && user.PasswordHash is null)
                return Result.Failure(DomainErrors.User.NoPasswordSet);

            //verify the password
            if (!hasher.Verify(user.PasswordHash, cmd.CurrentPassword))
            {
                return Result.Failure(DomainErrors.User.WrongPassword);
            }

            var newHash = hasher.Hash(cmd.NewPassword);
            user.UpdatePassword(newHash);

            await uow.SaveChangesAsync(ct);
            return Result.Success();



        }

    }
}
