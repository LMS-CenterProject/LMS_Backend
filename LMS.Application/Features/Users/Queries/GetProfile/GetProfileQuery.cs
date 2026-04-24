using LMS.Application.Common.Models;
using LMS.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Users.Queries.GetProfile
{
    public sealed record GetProfileQuery : IRequest<Result<UserProfileDto>>;
}
