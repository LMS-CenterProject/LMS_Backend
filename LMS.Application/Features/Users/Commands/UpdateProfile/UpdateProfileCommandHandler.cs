using LMS.Application.Common.Interfaces;
using LMS.Application.Common.Models;
using LMS.Application.DTOs;
using LMS.Domain.Errors;
using LMS.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Users.Commands.UpdateProfile
{
    public sealed class UpdateProfileCommandHandler(
        IUnitOfWork uow,ICurrentUserService current) : IRequestHandler<UpdateProfileCommand, Result<UserProfileDto>>
    {
        public async Task<Result<UserProfileDto>> Handle(UpdateProfileCommand cmd, CancellationToken ct)
        {
            var user = await uow.Users.GetByIdAsync(current.UserId.Value, ct);
            if (user == null)
            {
                return Result.Failure<UserProfileDto>(DomainErrors.User.NotFound);
            }
          
            user.UpdateProfile(cmd.FullName, cmd.PhoneNumber, cmd.AvatarUrl);
            await uow.SaveChangesAsync(ct);
            return Result.Success(new UserProfileDto(
                 Id: user.Id,
                 FullName: user.FullName,
                 Email: user.Email,
                 PhoneNumber: user.PhoneNumber,
                 AvatarUrl: user.AvatarUrl,
                 Role: user.Role.ToString(),
                 AuthProvider: user.AuthProvider.ToString(),
                 IsActive: user.IsActive,
                 CreatedAt: user.CreatedAt));
        }

    }
}
