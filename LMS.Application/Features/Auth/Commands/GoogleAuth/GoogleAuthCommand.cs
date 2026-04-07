using LMS.Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Auth.Commands.GoogleAuth
{
    public sealed record GoogleAuthCommand(string IdToken)
    : IRequest<Result<AuthResponse>>;
}
