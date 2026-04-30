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
        IUnitOfWork uow, ICurrentUserService current) : IRequestHandler<UpdateProfileCommand, Result<UserProfileDto>>
    {
        public async Task<Result<UserProfileDto>> Handle(UpdateProfileCommand cmd, CancellationToken ct)
        {
            if (!current.UserId.HasValue)
                return Result.Failure<UserProfileDto>(DomainErrors.User.NotFound);

            var user = await uow.Users.GetByIdAsync(current.UserId.Value, ct);
            if (user == null)
            {
                return Result.Failure<UserProfileDto>(DomainErrors.User.NotFound);
            }

            // Only update provided fields
            var fullName = cmd.FullName ?? user.FullName;
            var phoneNumber = cmd.PhoneNumber ?? user.PhoneNumber;
            var avatarUrl = cmd.AvatarUrl ?? user.AvatarUrl;
            
            user.UpdateProfile(fullName, phoneNumber, avatarUrl);
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
