using LMS.Application.Common.Models;
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
    string? PhoneNumber) : IRequest<Result<AuthResponse>>;
}
