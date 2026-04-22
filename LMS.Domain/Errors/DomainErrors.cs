namespace LMS.Domain.Errors
{
    public static class DomainErrors
    {
        public static class User
        {
            public static readonly Error NotFound = new("User.NotFound", "User was not found.");
            public static readonly Error EmailAlreadyExists = new("User.EmailAlreadyExists", "A user with this email already exists.");
            public static readonly Error InvalidCredentials = new("User.InvalidCredentials", "Email or password is incorrect.");
            public static readonly Error AccountDisabled = new("User.AccountDisabled", "This account has been deactivated.");
            public static readonly Error CannotDeactivateSelf = new("User.CannotDeactivateSelf", "You cannot deactivate your own account.");
            public static readonly Error GoogleIdAlreadyLinked = new("User.GoogleIdAlreadyLinked", "This Google account is already linked to another user.");
        }

        public static class Course
        {
            public static readonly Error NotFound = new("Course.NotFound", "Course was not found.");
            public static readonly Error NotPublished = new("Course.NotPublished", "This course is not available for enrollment.");
            public static readonly Error Unauthorized = new("Course.Unauthorized", "You do not own this course.");
            public static readonly Error AlreadyPublished = new("Course.AlreadyPublished", "Course is already published.");
            public static readonly Error CannotPublishEmpty = new("Course.CannotPublishEmpty", "Cannot publish a course with no lessons.");
        }

        public static class Enrollment
        {
            public static readonly Error NotFound = new("Enrollment.NotFound", "Enrollment was not found.");
            public static readonly Error AlreadyExists = new("Enrollment.AlreadyExists", "You are already enrolled in this course.");
            public static readonly Error NotActive = new("Enrollment.NotActive", "This enrollment is not active.");
            public static readonly Error Unauthorized = new("Enrollment.Unauthorized", "This enrollment does not belong to you.");
        }

        public static class Lesson
        {
            public static readonly Error NotFound = new("Lesson.NotFound", "Lesson was not found.");
            public static readonly Error NotAccessible = new("Lesson.NotAccessible", "You must be enrolled to access this lesson.");
        }

        public static class Quiz
        {
            public static readonly Error NotFound = new("Quiz.NotFound", "Quiz was not found.");
            public static readonly Error NotEligible = new("Quiz.NotEligible", "You must be enrolled to attempt this quiz.");
            public static readonly Error NoCorrectAnswer = new("Quiz.NoCorrectAnswer", "Each question must have at least one correct answer.");
            public static readonly Error InvalidSingleChoice = new("Quiz.InvalidSingleChoice", "A single-choice question can only have one correct answer.");
        }

        public static class Review
        {
            public static readonly Error AlreadyExists = new("Review.AlreadyExists", "You have already reviewed this.");
            public static readonly Error NotEligible = new("Review.NotEligible", "You must be enrolled to leave a review.");
            public static readonly Error LessonNotCompleted = new("Review.LessonNotCompleted", "You must complete the lesson before reviewing it.");
            public static readonly Error CannotReviewOwn = new("Review.CannotReviewOwn", "You cannot review your own content.");
            public static readonly Error InvalidRating = new("Review.InvalidRating", "Rating must be between 1 and 5.");
            public static readonly Error NotFound = new("Review.NotFound", "Review was not found.");
            public static readonly Error Unauthorized = new("Review.Unauthorized", "This review does not belong to you.");
            public static readonly Error EditWindowClosed = new("Review.EditWindowClosed", "Reviews can only be edited within 30 days of posting.");

        }

        public static class Token
        {
            public static readonly Error Invalid = new("Token.Invalid", "Token is invalid or has expired.");
            public static readonly Error Revoked = new("Token.Revoked", "Token has been revoked.");
        }

        public static class LessonProgress
        {
            public static readonly Error NotFound = new("LessonProgress.NotFound", "Lesson progress record was not found.");
            public static readonly Error NotEnrolled = new("LessonProgress.NotEnrolled", "You are not enrolled in this course.");
            public static readonly Error AlreadyDone = new("LessonProgress.AlreadyDone", "This lesson is already marked as completed.");
        }

        public static class Certificate
        {
            public static readonly Error NotFound = new("Certificate.NotFound", "Certificate was not found.");
        }
    }
}
