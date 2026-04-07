using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Common.Models
{
    public sealed record AuthResponse(
    Guid UserId,
    string FullName,
    string Email,
    string Role,
    string AccessToken,
    string RefreshToken);
}
