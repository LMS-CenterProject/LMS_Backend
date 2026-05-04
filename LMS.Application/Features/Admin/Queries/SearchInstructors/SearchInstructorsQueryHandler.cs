using LMS.Application.Common.Models;
using LMS.Application.DTOs;
using LMS.Domain.Enums;
using LMS.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Admin.Queries.SearchInstructors
{
    public sealed class SearchInstructorsQueryHandler(IUnitOfWork uow)
        : IRequestHandler<SearchInstructorsQuery, Result<PagedUsersDto>>
    {
        private const int MaxPageSize = 50;

        public async Task<Result<PagedUsersDto>> Handle(
            SearchInstructorsQuery query, CancellationToken ct)
        {
            var page = Math.Max(query.Page, 1);
            var pageSize = Math.Clamp(query.PageSize, 1, MaxPageSize);

            // Filter by Instructor role only
            var (users, totalCount) = await uow.Users.GetAllPaginatedAsync(
                page,
                pageSize,
                query.Search,
                role: UserRole.Instructor,
                ct);

            var totalPages = totalCount == 0 ? 0
                : (int)Math.Ceiling((double)totalCount / pageSize);

            var dtos = users.Select(u => new UserListDto(
                Id: u.Id,
                FullName: u.FullName,
                Email: u.Email,
                PhoneNumber: u.PhoneNumber,
                Role: u.Role.ToString(),
                AuthProvider: u.AuthProvider.ToString(),
                IsActive: u.IsActive,
                CreatedAt: u.CreatedAt));

            return Result.Success(new PagedUsersDto(
                Users: dtos,
                TotalCount: totalCount,
                Page: page,
                PageSize: pageSize,
                TotalPages: totalPages));
        }
    }
}
