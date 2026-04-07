using Google.Apis.Auth;
using LMS.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Infrastructure.Services.Auth
{
    public sealed class GoogleAuthService(IConfiguration config) : IGoogleAuthService
    {
        public async Task<GoogleUserInfo?> ValidateAsync(
            string idToken, CancellationToken ct = default)
        {
            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = [config["Google:ClientId"]!]
                };

                var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);

                return new GoogleUserInfo(
                    GoogleId: payload.Subject,
                    Email: payload.Email,
                    FullName: payload.Name,
                    AvatarUrl: payload.Picture);
            }
            catch
            {
                // Invalid token, expired, wrong audience — all treated the same
                return null;
            }
        }
    }
}
