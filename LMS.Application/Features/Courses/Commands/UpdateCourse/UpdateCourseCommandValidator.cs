using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Courses.Commands.UpdateCourse
{
    public sealed class UpdateCourseCommandValidator:AbstractValidator<UpdateCourseCommand>
    {
        public UpdateCourseCommandValidator() {
            RuleFor(x => x.CourseId)
                .NotEmpty();

            RuleFor(x => x.Title)
                .NotEmpty()
                .MaximumLength(200);

            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.Language)
                .NotEmpty();
        }
    }

}
