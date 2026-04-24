using LMS.Application.Common.Models;
using LMS.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Users.Commands.UpdateProfile
{
    public sealed record UpdateProfileCommand(
       string FullName,
       string? PhoneNumber,
       string? AvatarUrl) : IRequest<Result<UserProfileDto>>;
}
