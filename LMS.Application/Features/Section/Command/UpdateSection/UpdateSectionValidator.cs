using FluentValidation;

namespace LMS.Application.Features.Section.Command.UpdateSection
{
    public sealed class UpdateSectionValidator:AbstractValidator<UpdateSectionCommand>
    {
        public UpdateSectionValidator()
        {
            RuleFor(x => x.SectionId).NotEmpty();
            RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
            RuleFor(x => x.OrderIndex).NotEmpty();
        }
    }
}
