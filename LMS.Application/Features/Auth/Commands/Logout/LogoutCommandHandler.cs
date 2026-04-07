using LMS.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Auth.Commands.Logout
{
    public sealed class LogoutCommandHandler(IUnitOfWork uow)
    : IRequestHandler<LogoutCommand>
    {
        public async Task Handle(LogoutCommand cmd, CancellationToken ct)
        {
            var token = await uow.RefreshTokens.GetByTokenAsync(cmd.RefreshToken, ct);

            // Silently succeed even if token not found — idempotent logout
            if (token is null || !token.IsActive) return;

            token.Revoke();
            await uow.SaveChangesAsync(ct);
        }
    }
}
