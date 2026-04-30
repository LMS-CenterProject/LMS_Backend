using LMS.Application.Common.Models;
using LMS.Application.DTOs;
using LMS.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Users.Queries.GetAllUsers
{
    public sealed record GetAllUsersQuery(
        int Page = 1,
        int PageSize = 10,
        string? Search = null,
        UserRole? Role = null) : IRequest<Result<PagedUsersDto>>;
}
