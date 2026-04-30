using LMS.Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Notifications.Commands
{
    public sealed record MarkAllReadCommand : IRequest<Result<int>>;

}
