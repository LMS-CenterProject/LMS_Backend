namespace LMS.Application.Courses.DTOs
{
    public class CreateCourseResponse
    {
        public string Message { get; set; } = string.Empty;
        public Guid CourseId { get; set; }
        public string Title { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Status { get; set; } = "Draft";

        public CreateCourseResponse(Guid courseId, string title, decimal price)
        {
            CourseId = courseId;
            Title = title;
            Price = price;
            Message = "Course created successfully";
            Status = "Draft";
        }
    }
}