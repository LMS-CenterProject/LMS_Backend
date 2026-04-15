using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Lesson.Command.AddLesson
{
    public sealed class AddLessonValidator:AbstractValidator<AddLessonCommand>
    {
        public AddLessonValidator()
        {
            RuleFor(x => x.SectionId)
                .NotEmpty();
        }
    }
}
