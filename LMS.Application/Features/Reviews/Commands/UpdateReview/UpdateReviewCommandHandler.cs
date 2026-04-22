using LMS.Application.Common.Interfaces;
using LMS.Application.Common.Models;
using LMS.Application.Features.Reviews.Commands.UpdateReview.LMS.Application.Features.Reviews.Commands.UpdateReview;
using LMS.Domain.Errors;
using LMS.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Reviews.Commands.UpdateReview
{
    public sealed class UpdateReviewCommandHandler(
       IUnitOfWork uow,
       ICurrentUserService current)
       : IRequestHandler<UpdateReviewCommand, Result>
    {
        private static readonly TimeSpan EditWindow = TimeSpan.FromDays(30);

        public async Task<Result> Handle(
            UpdateReviewCommand cmd, CancellationToken ct)
        {
            // 1. Find review
            var review = await uow.Reviews.GetByIdAsync(cmd.ReviewId, ct);

            if (review is null)
                return Result.Failure(DomainErrors.Review.NotFound);

            // 2. Ownership check
            if (review.StudentId != current.UserId)
                return Result.Failure(DomainErrors.Review.Unauthorized);

            // 3. 30-day edit window
            if (DateTime.UtcNow - review.CreatedAt > EditWindow)
                return Result.Failure(DomainErrors.Review.EditWindowClosed);

            // 4. Update (domain method validates rating)
            review.Update(cmd.Rating, cmd.Comment);
            await uow.SaveChangesAsync(ct);

            return Result.Success();
        }
    }
}
