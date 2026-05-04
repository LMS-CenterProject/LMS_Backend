using LMS.Application.Common.Models;
using LMS.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Admin.Queries.SearchInstructors
{
    public sealed record SearchInstructorsQuery(
        string? Search = null,
        int Page = 1,
        int PageSize = 20) : IRequest<Result<PagedUsersDto>>;
}
