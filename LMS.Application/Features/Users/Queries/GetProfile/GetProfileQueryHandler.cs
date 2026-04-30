using LMS.Application.Common.Interfaces;
using LMS.Application.Common.Models;
using LMS.Application.DTOs;
using LMS.Domain.Errors;
using LMS.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Users.Queries.GetProfile
{
    public sealed class GetProfileQueryHandler(
        IUnitOfWork uow,
        ICurrentUserService current)
        : IRequestHandler<GetProfileQuery, Result<UserProfileDto>>
    {

        public async Task<Result<UserProfileDto>> Handle(GetProfileQuery request, CancellationToken ct)
        {
            var user = await uow.Users.GetByIdAsync(current.UserId.Value, ct);
            if (user == null)
            {
                return Result.Failure<UserProfileDto>(DomainErrors.User.NotFound);
            }
            return Result.Success(new UserProfileDto(
                user.Id,
                user.FullName,
                user.Email,
                user.PhoneNumber,
                user.AvatarUrl,
                user.Role.ToString(),
                user.AuthProvider.ToString(),
                user.IsActive,
                user.CreatedAt));
        }
    }
}
