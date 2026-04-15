using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Courses.Commands.PublishCourse
{
    public sealed class PublishCourseValidator:AbstractValidator<PublishCourseCommand>
    {
        public PublishCourseValidator() {
            RuleFor(x => x.CourseId).NotEmpty();
        }
    }
}
