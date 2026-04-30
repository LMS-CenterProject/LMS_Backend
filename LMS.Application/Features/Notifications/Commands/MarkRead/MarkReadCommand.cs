using LMS.Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Notifications.Commands.MarkRead
{
    public sealed record MarkReadCommand(Guid NotificationId) : IRequest<Result>;

}
