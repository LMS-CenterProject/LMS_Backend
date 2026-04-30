using LMS.Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Reviews.Commands.UpdateReview
{
    namespace LMS.Application.Features.Reviews.Commands.UpdateReview
    {
        public sealed record UpdateReviewCommand(
            Guid ReviewId,
            byte Rating,
            string? Comment) : IRequest<Result>;
    }
}
