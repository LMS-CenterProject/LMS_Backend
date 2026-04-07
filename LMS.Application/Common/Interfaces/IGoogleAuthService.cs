using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Common.Interfaces
{
    public sealed record GoogleUserInfo(
    string GoogleId,
    string Email,
    string FullName,
    string? AvatarUrl);

    public interface IGoogleAuthService
    {
        Task<GoogleUserInfo?> ValidateAsync(string idToken, CancellationToken ct = default);
    }
}
