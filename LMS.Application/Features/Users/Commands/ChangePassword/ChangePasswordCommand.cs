using LMS.Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Users.Commands.ChangePassword
{
    public sealed record ChangePasswordCommand(string CurrentPassword, string NewPassword) : IRequest<Result>;
    
}
