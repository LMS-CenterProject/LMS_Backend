using FluentValidation;
using LMS.Application.Features.Reviews.Commands.UpdateReview.LMS.Application.Features.Reviews.Commands.UpdateReview;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Reviews.Commands.UpdateReview
{
    public sealed class UpdateReviewCommandValidator
        : AbstractValidator<UpdateReviewCommand>
    {
        public UpdateReviewCommandValidator()
        {
            RuleFor(x => x.ReviewId)
                .NotEmpty();

            RuleFor(x => x.Rating)
                .InclusiveBetween((byte)1, (byte)5)
                .WithMessage("Rating must be between 1 and 5.");

            RuleFor(x => x.Comment)
                .MaximumLength(2000)
                .When(x => x.Comment is not null);
        }
    }
}
