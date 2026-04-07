using LMS.Application.Common.Interfaces;
using LMS.Application.Common.Models;
using LMS.Domain.Errors;
using LMS.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Auth.Commands.Login
{
    public sealed class LoginCommandHandler(
    IUnitOfWork uow,
    IPasswordHasher hasher,
    IJwtService jwt,
    IConfiguration config)
    : IRequestHandler<LoginCommand, Result<AuthResponse>>
    {
        public async Task<Result<AuthResponse>> Handle(
            LoginCommand cmd, CancellationToken ct)
        {
            // 1. Find user — same error for wrong email OR wrong password (security)
            var user = await uow.Users.GetByEmailAsync(cmd.Email, ct);

            if (user is null || !hasher.Verify(cmd.Password, user.PasswordHash ?? string.Empty))
                return Result.Failure<AuthResponse>(DomainErrors.User.InvalidCredentials);

            // 2. Check account is active
            if (!user.IsActive)
                return Result.Failure<AuthResponse>(DomainErrors.User.AccountDisabled);

            // 3. Issue tokens
            var accessToken = jwt.GenerateAccessToken(user);
            var rawRefresh = jwt.GenerateRefreshToken();
            var expiryDays = int.Parse(config["Jwt:RefreshTokenExpiryDays"] ?? "7");
            var refreshToken = LMS.Domain.Entities.RefreshToken.Create(user.Id, rawRefresh, expiryDays);

            await uow.RefreshTokens.AddAsync(refreshToken, ct);
            await uow.SaveChangesAsync(ct);

            return Result.Success(new AuthResponse(
                user.Id, user.FullName, user.Email,
                user.Role.ToString(), accessToken, rawRefresh));
        }
    }
}
