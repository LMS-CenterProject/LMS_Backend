using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Auth.Commands.Logout
{
    public sealed record LogoutCommand(string RefreshToken) : IRequest;
}
