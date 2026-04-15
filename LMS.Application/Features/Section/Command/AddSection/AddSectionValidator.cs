using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Section.Command.CreateSection
{
    public sealed class AddSectionValidator:AbstractValidator<AddSectionCommand>
    {
        public AddSectionValidator()
        {
            RuleFor(x => x.CourseId)
                .NotEmpty();

            RuleFor(x => x.Title)
                .NotEmpty()
                .MaximumLength(200);

            RuleFor(x => x.OrderIndex)
                .GreaterThanOrEqualTo(1);
        }
    }
}
