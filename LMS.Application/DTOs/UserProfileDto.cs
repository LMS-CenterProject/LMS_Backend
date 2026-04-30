using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.DTOs
{
    public sealed record UserProfileDto(
        Guid Id,
        string FullName,
        string Email,
        string? PhoneNumber,
        string? AvatarUrl,
        string Role,
        string AuthProvider,
        bool IsActive,
        DateTime CreatedAt);

    public sealed record UserListDto(
        Guid Id,
        string FullName,
        string Email,
        string? PhoneNumber,
        string Role,
        string AuthProvider,
        bool IsActive,
        DateTime CreatedAt);

    public sealed record PagedUsersDto(
        IEnumerable<UserListDto> Users,
        int TotalCount,
        int Page,
        int PageSize,
        int TotalPages);
}
