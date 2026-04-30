using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Users.Commands.ChangePassword
{
    public sealed class ChangePasswordCommandValidator
        : AbstractValidator<ChangePasswordCommand>
    {
        public ChangePasswordCommandValidator()
        {
            RuleFor(x => x.CurrentPassword)
                .NotEmpty().WithMessage("Current password is required.");

            RuleFor(x => x.NewPassword)
                .NotEmpty()
                .MinimumLength(8).WithMessage("New password must be at least 8 characters.")
                .Matches("[A-Z]").WithMessage("Must contain an uppercase letter.")
                .Matches("[0-9]").WithMessage("Must contain a digit.")
                .Matches("[^a-zA-Z0-9]").WithMessage("Must contain a special character.")
                .NotEqual(x => x.CurrentPassword)
                .WithMessage("New password must be different from current password.");
        }
    }
}
