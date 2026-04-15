using LMS.Application.DTOs.Lesson;

public class SectionDto
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public int OrderIndex { get; set; }

    public List<LessonDto> Lessons { get; set; } = [];
}