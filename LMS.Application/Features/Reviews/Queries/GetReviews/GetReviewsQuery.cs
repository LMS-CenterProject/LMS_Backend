using LMS.Application.Common.Models;
using LMS.Application.DTOs;
using LMS.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Reviews.Queries.GetReviews
{
    public sealed record GetReviewsQuery(
        Guid TargetId,
        ReviewTargetType TargetType) : IRequest<Result<ReviewsResultDto>>;
}
