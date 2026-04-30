using LMS.Application.Common.Models;
using LMS.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Notifications.Queries.GetNotifications
{
    public sealed record GetNotificationsQuery(
       int Page = 1,
       int PageSize = 20,
       bool OnlyUnread = false) : IRequest<Result<NotificationsResultDto>>;
}
