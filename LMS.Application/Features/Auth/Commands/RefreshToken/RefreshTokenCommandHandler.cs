using LMS.Application.Common.Interfaces;
using LMS.Application.Common.Models;
using LMS.Application.Common.Settings;
using LMS.Domain.Errors;
using LMS.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Options;
using LMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Auth.Commands.RefreshToken
{
    public sealed class RefreshTokenCommandHandler(
    IUnitOfWork uow,
    IJwtService jwt,
    IOptions<JwtSettings> jwtSettings)
    : IRequestHandler<RefreshTokenCommand, Result<AuthResponse>>
    {
        private readonly JwtSettings _jwt = jwtSettings.Value;

        public async Task<Result<AuthResponse>> Handle(
            RefreshTokenCommand cmd, CancellationToken ct)
        {
            var existing = await uow.RefreshTokens.GetByTokenAsync(cmd.RefreshToken, ct);

            if (existing is null)
                return Result.Failure<AuthResponse>(DomainErrors.Token.Invalid);

            if (existing.IsRevoked)
                return Result.Failure<AuthResponse>(DomainErrors.Token.Revoked);

            if (existing.IsExpired)
                return Result.Failure<AuthResponse>(DomainErrors.Token.Invalid);

            var newAccessToken = jwt.GenerateAccessToken(existing.User);
            var newRawRefresh = jwt.GenerateRefreshToken();
            var newRefreshToken = LMS.Domain.Entities.RefreshToken.Create(
                existing.UserId, newRawRefresh, _jwt.RefreshTokenExpiryDays);

            existing.Revoke(replacedByToken: newRawRefresh);

            await uow.RefreshTokens.AddAsync(newRefreshToken, ct);
            await uow.SaveChangesAsync(ct);

            var user = existing.User;
            return Result.Success(new AuthResponse(
                user.Id, user.FullName, user.Email,
                user.Role.ToString(), newAccessToken, newRawRefresh));
        }
    }
}
