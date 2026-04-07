using LMS.Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Auth.Commands.Login
{
    public sealed record LoginCommand(
    string Email,
    string Password) : IRequest<Result<AuthResponse>>;
    //IRequest<T> from MediatR=>This command will be handled, and it will return a response of type Result<AuthResponse>
}
