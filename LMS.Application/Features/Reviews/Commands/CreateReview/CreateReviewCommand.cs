using LMS.Application.Common.Models;
using LMS.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Reviews.Commands.CreateReview
{
    public sealed record CreateReviewCommand(
        Guid TargetId,
        ReviewTargetType TargetType,
        byte Rating,
        string? Comment) : IRequest<Result<Guid>>;
}
