using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Reviews.Commands.CreateReview
{
    public sealed class CreateReviewCommandValidator
       : AbstractValidator<CreateReviewCommand>
    {
        public CreateReviewCommandValidator()
        {
            RuleFor(x => x.TargetId)
                .NotEmpty().WithMessage("TargetId is required.");

            RuleFor(x => x.TargetType)
                .IsInEnum().WithMessage("TargetType must be Course (0), Teacher (1), or Lesson (2).");

            RuleFor(x => x.Rating)
                .InclusiveBetween((byte)1, (byte)5)
                .WithMessage("Rating must be between 1 and 5.");

            RuleFor(x => x.Comment)
                .MaximumLength(2000)
                .WithMessage("Comment must not exceed 2000 characters.")
                .When(x => x.Comment is not null);
        }
    }
}
