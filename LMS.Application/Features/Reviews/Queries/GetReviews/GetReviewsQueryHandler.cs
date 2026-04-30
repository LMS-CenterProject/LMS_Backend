using LMS.Application.Common.Models;
using LMS.Application.DTOs;
using LMS.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Reviews.Queries.GetReviews
{
    public sealed class GetReviewsQueryHandler(IUnitOfWork uow)
        : IRequestHandler<GetReviewsQuery, Result<ReviewsResultDto>>
    {
        public async Task<Result<ReviewsResultDto>> Handle(
            GetReviewsQuery query, CancellationToken ct)
        {
            var reviews = (await uow.Reviews.GetByTargetAsync(
                query.TargetId, query.TargetType, ct)).ToList();

            var avgRating = await uow.Reviews.GetAverageRatingAsync(
                query.TargetId, query.TargetType, ct);

            var dtos = reviews.Select(r => new ReviewDto(
                Id: r.Id,
                StudentId: r.StudentId,
                StudentName: r.Student.FullName,
                TargetId: r.TargetId,
                TargetType: r.TargetType.ToString(),
                Rating: r.Rating,
                Comment: r.Comment,
                CreatedAt: r.CreatedAt,
                UpdatedAt: r.UpdatedAt));

            return Result.Success(new ReviewsResultDto(
                Reviews: dtos,
                AverageRating: Math.Round(avgRating, 1),
                TotalCount: reviews.Count));
        }
    }
}
