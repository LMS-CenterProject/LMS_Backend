using LMS.Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Auth.Commands.RefreshToken
{
    public sealed record RefreshTokenCommand(string RefreshToken)
    : IRequest<Result<AuthResponse>>;
}
