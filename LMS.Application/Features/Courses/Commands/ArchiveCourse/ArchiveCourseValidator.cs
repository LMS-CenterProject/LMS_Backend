using FluentValidation;


namespace LMS.Application.Features.Courses.Commands.ArchiveCourse
{
    public class ArchiveCourseValidator:AbstractValidator<ArchiveCourseCommand>
    {
        public ArchiveCourseValidator()
        {
            RuleFor(x => x.CourseId).NotEmpty();
        }
    }
}
