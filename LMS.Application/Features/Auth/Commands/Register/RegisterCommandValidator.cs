using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;

namespace LMS.Application.Features.Auth.Commands.Register
{
    public sealed class RegisterCommandValidator : AbstractValidator<RegisterCommand>
    {
        public RegisterCommandValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Full name is required.")
                .MaximumLength(150);

            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress().WithMessage("A valid email is required.")
                .MaximumLength(256);

            RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(8).WithMessage("Password must be at least 8 characters.")
                .Matches("[A-Z]").WithMessage("Must contain an uppercase letter.")
                .Matches("[0-9]").WithMessage("Must contain a digit.");

            RuleFor(x => x.PhoneNumber)
                .MinimumLength(7).WithMessage("Phone number must be at least 7 digits.")
                .When(x => x.PhoneNumber is not null);
        }
    }
}
