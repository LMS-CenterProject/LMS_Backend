using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Users.Commands.UpdateProfile
{
    public sealed class UpdateProfileCommandValidator
        : AbstractValidator<UpdateProfileCommand>
    {
        public UpdateProfileCommandValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Full name is required.")
                .MaximumLength(150).WithMessage("Full name must not exceed 150 characters.")
                .When(x => x.FullName is not null);

            RuleFor(x => x.PhoneNumber)
                .MinimumLength(7).WithMessage("Phone number must be at least 7 digits.")
                .MaximumLength(20).WithMessage("Phone number must not exceed 20 characters.")
                .When(x => x.PhoneNumber is not null);
        }
    }
}
