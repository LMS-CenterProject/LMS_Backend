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

namespace LMS.Application.Features.Auth.Commands.GoogleAuth
{
    public sealed class GoogleAuthCommandHandler(
    IUnitOfWork uow,
    IGoogleAuthService google,
    IJwtService jwt,
    IConfiguration config)
    : IRequestHandler<GoogleAuthCommand, Result<AuthResponse>>
    {
        public async Task<Result<AuthResponse>> Handle(
            GoogleAuthCommand cmd, CancellationToken ct)
        {
            // 1. Validate Google token — returns null if invalid/expired
            var googleUser = await google.ValidateAsync(cmd.IdToken, ct);

            if (googleUser is null)
                return Result.Failure<AuthResponse>(DomainErrors.Token.Invalid);

            // 2. Look up user: first by GoogleId, then by email (account linking)
            var user = await uow.Users.GetByGoogleIdAsync(googleUser.GoogleId, ct)
                    ?? await uow.Users.GetByEmailAsync(googleUser.Email, ct);

            if (user is null)
            {
                // 3a. New user — create from Google profile
                user = User.CreateWithGoogle(
                    googleUser.FullName,
                    googleUser.Email,
                    googleUser.GoogleId,
                    googleUser.AvatarUrl);

                await uow.Users.AddAsync(user, ct);
            }
            else
            {
                // 3b. Existing user — link Google account if not already linked
                if (user.GoogleId is null)
                    user.LinkGoogle(googleUser.GoogleId);

                if (!user.IsActive)
                    return Result.Failure<AuthResponse>(DomainErrors.User.AccountDisabled);
            }

            // 4. Issue tokens
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
