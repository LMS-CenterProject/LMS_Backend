using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Progress.Commands.UpdateLessonProgress
{
    public sealed class UpdateLessonProgressCommandValidator
    : AbstractValidator<UpdateLessonProgressCommand>
    {
        public UpdateLessonProgressCommandValidator()
        {
            RuleFor(x => x.LessonId)
                .NotEmpty().WithMessage("LessonId is required.");

            RuleFor(x => x.WatchedSeconds)
                .GreaterThanOrEqualTo(0)
                .WithMessage("WatchedSeconds must be 0 or greater.");
        }
    }
}
