using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Admin.Command.ChangeUserRole
{
    public sealed class ChangeUserRoleCommandValidator
        : AbstractValidator<ChangeUserRoleCommand>
    {
        public ChangeUserRoleCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId is required.");

            RuleFor(x => x.NewRole)
                .IsInEnum().WithMessage("Role must be Student(0), Instructor(1), Admin(2), or SuperAdmin(3).");
        }
    }
}
