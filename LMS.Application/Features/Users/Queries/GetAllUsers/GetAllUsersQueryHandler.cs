using LMS.Application.Common.Models;
using LMS.Application.DTOs;
using LMS.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Users.Queries.GetAllUsers
{
    public sealed class GetAllUsersQueryHandler(IUnitOfWork uow)
        : IRequestHandler<GetAllUsersQuery, Result<PagedUsersDto>>
    {
        private const int MaxPageSize = 50;

        public async Task<Result<PagedUsersDto>> Handle(
            GetAllUsersQuery query, CancellationToken ct)
        {
            // Cap page size — never let client request unlimited data
            var pageSize = Math.Min(query.PageSize, MaxPageSize);
            var page = Math.Max(query.Page, 1);

            var (users, totalCount) = await uow.Users.GetAllPaginatedAsync(
                page, pageSize, query.Search, query.Role, ct);

            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

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
