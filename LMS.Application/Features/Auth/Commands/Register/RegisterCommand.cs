using LMS.Application.Common.Models;
using LMS.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Auth.Commands.Register
{
    public sealed record RegisterCommand(
    string FullName,
    string Email,
    string Password,
    string? PhoneNumber,
    UserRole Role = UserRole.Student) : IRequest<Result<AuthResponse>>;
}
