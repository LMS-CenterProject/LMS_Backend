using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Courses.Commands.CreateCourse
{
    public sealed class CreateCoureCommandValidator : AbstractValidator<CreateCourseCommand>
    {
        public CreateCoureCommandValidator()
        {
            // InstructorId is optional — only validated if provided
            RuleFor(x => x.InstructorId)
                .NotEmpty()
                .WithMessage("InstructorId must be a valid non-empty GUID when provided.")
                .When(x => x.InstructorId.HasValue);

            RuleFor(x => x.CategoryId)
                .NotEmpty().WithMessage("CategoryId is required.");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Course title is required.")
                .MaximumLength(200).WithMessage("Course title must not exceed 200 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(1000)
                .WithMessage("Course description must not exceed 1000 characters.");

            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Price must be a non-negative value.");

            RuleFor(x => x.Language)
                .NotEmpty().WithMessage("Language is required.")
                .MaximumLength(50).WithMessage("Language must not exceed 50 characters.");
        }
    }
}
