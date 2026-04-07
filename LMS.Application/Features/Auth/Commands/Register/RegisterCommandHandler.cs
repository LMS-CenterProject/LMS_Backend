using LMS.Application.Common.Interfaces;
using LMS.Application.Common.Models;
using LMS.Domain.Entities;
using LMS.Domain.Errors;
using LMS.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Auth.Commands.Register
{
    public sealed class RegisterCommandHandler(
    IUnitOfWork uow,
    IPasswordHasher hasher,
    IJwtService jwt,
    IConfiguration config)
    : IRequestHandler<RegisterCommand, Result<AuthResponse>>
    {
        public async Task<Result<AuthResponse>> Handle(
            RegisterCommand cmd, CancellationToken ct)
        {
            // 1. Check duplicate email
            if (await uow.Users.EmailExistsAsync(cmd.Email, ct))
                return Result.Failure<AuthResponse>(DomainErrors.User.EmailAlreadyExists);

            // 2. Create user
            var user = User.CreateLocal(
                cmd.FullName,
                cmd.Email,
                hasher.Hash(cmd.Password),
                cmd.PhoneNumber);

            await uow.Users.AddAsync(user, ct);

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
