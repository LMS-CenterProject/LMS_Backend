using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.DTOs
{
    public sealed record ReviewDto(
       Guid Id,
       Guid StudentId,
       string StudentName,
       Guid TargetId,
       string TargetType,
       byte Rating,
       string? Comment,
       DateTime CreatedAt,
       DateTime? UpdatedAt);

    public sealed record ReviewsResultDto(
        IEnumerable<ReviewDto> Reviews,
        double AverageRating,
        int TotalCount);
}
