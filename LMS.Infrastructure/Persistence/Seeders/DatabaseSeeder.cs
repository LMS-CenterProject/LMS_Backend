using LMS.Domain.Entities;
using LMS.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LMS.Infrastructure.Persistence.Seeders;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(LMSDbContext context, ILogger logger)
    {
        try
        {
            await SeedUsersAsync(context, logger);
            await SeedCategoriesAsync(context, logger);
            await SeedTagsAsync(context, logger);
            await SeedCoursesAsync(context, logger);
            await SeedQuizzesAsync(context, logger);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    // ── Users ─────────────────────────────────────────────────────

    private static async Task SeedUsersAsync(LMSDbContext context, ILogger logger)
    {
        if (await context.Users.AnyAsync())
        {
            logger.LogInformation("Users already seeded — skipping.");
            return;
        }

        var users = new List<User>
        {
            CreateUser("Super Admin",     "superadmin@lms.com",       "SuperAdmin@123",  UserRole.SuperAdmin,  "+1000000000"),
            CreateUser("Admin User",      "admin@lms.com",            "Admin@123",       UserRole.Admin,       "+1000000001"),
            CreateUser("Ahmed Hassan",    "ahmed.hassan@lms.com",     "Instructor@123",  UserRole.Instructor,  "+201001234567"),
            CreateUser("Sarah Johnson",   "sarah.johnson@lms.com",    "Instructor@123",  UserRole.Instructor,  "+1234567890"),
            CreateUser("Mohamed Ali",     "mohamed.ali@lms.com",      "Instructor@123",  UserRole.Instructor,  "+201112345678"),
            CreateUser("Omar Khaled",     "omar.khaled@lms.com",      "Student@123",     UserRole.Student,     "+201234567890"),
            CreateUser("Nour Ibrahim",    "nour.ibrahim@lms.com",     "Student@123",     UserRole.Student,     "+201345678901"),
            CreateUser("Layla Ahmed",     "layla.ahmed@lms.com",      "Student@123",     UserRole.Student,     "+201456789012"),
            CreateUser("Youssef Mostafa", "youssef.mostafa@lms.com",  "Student@123",     UserRole.Student,     "+201567890123"),
            CreateUser("Hana Sami",       "hana.sami@lms.com",        "Student@123",     UserRole.Student,     "+201678901234"),
        };

        await context.Users.AddRangeAsync(users);
        await context.SaveChangesAsync();

        logger.LogInformation("Seeded {Count} users.", users.Count);
    }

    // ── Categories ─────────────────────────────────────────────────

    private static async Task SeedCategoriesAsync(LMSDbContext context, ILogger logger)
    {
        if (await context.Categories.AnyAsync())
        {
            logger.LogInformation("Categories already seeded — skipping.");
            return;
        }

        var development = Category.Create("Development");
        var design = Category.Create("Design");
        var business = Category.Create("Business");
        var dataScience = Category.Create("Data Science");

        await context.Categories.AddRangeAsync(development, design, business, dataScience);
        await context.SaveChangesAsync();

        var subCategories = new List<Category>
        {
            Category.Create("Web Development",  development.Id),
            Category.Create("Mobile",           development.Id),
            Category.Create("DevOps",           development.Id),
            Category.Create("UI/UX",            design.Id),
            Category.Create("Graphic Design",   design.Id),
            Category.Create("Machine Learning", dataScience.Id),
            Category.Create("Data Analysis",    dataScience.Id),
        };

        await context.Categories.AddRangeAsync(subCategories);
        await context.SaveChangesAsync();

        logger.LogInformation("Seeded categories.");
    }

    // ── Tags ───────────────────────────────────────────────────────

    private static async Task SeedTagsAsync(LMSDbContext context, ILogger logger)
    {
        if (await context.Tags.AnyAsync())
        {
            logger.LogInformation("Tags already seeded — skipping.");
            return;
        }

        var tags = new[]
        {
            "csharp", "dotnet", "aspnet", "sql", "efcore",
            "javascript", "react", "python", "api", "beginners"
        }.Select(Tag.Create).ToList();

        await context.Tags.AddRangeAsync(tags);
        await context.SaveChangesAsync();

        logger.LogInformation("Seeded {Count} tags.", tags.Count);
    }

    // ── Courses ────────────────────────────────────────────────────

    private static async Task SeedCoursesAsync(LMSDbContext context, ILogger logger)
    {
        if (await context.Courses.AnyAsync())
        {
            logger.LogInformation("Courses already seeded — skipping.");
            return;
        }

        // Load the instructors and categories we need
        var ahmed = await context.Users.FirstAsync(u => u.Email == "ahmed.hassan@lms.com");
        var sarah = await context.Users.FirstAsync(u => u.Email == "sarah.johnson@lms.com");
        var mohamed = await context.Users.FirstAsync(u => u.Email == "mohamed.ali@lms.com");

        var webDev = await context.Categories.FirstAsync(c => c.Name == "Web Development");
        var mobile = await context.Categories.FirstAsync(c => c.Name == "Mobile");
        var ml = await context.Categories.FirstAsync(c => c.Name == "Machine Learning");

        // ── Course 1: ASP.NET Core API (Published) ─────────────────
        var course1 = Course.Create(
            instructorId: ahmed.Id,
            categoryId: webDev.Id,
            title: "ASP.NET Core",
            description: "Build production-ready REST APIs with ASP.NET Core 8, EF Core, JWT authentication, and Clean Architecture from scratch.",
            price: 49.99m,
            level: CourseLevel.Intermediate,
            language: "English",
            thumbnailUrl: "main.png");

        course1.Publish();
        await context.Courses.AddAsync(course1);
        await context.SaveChangesAsync();

        await SeedCourse1Sections(context, course1.Id);

        // ── Course 2: Clean Architecture (Published) ───────────────
        var course2 = Course.Create(
            instructorId: ahmed.Id,
            categoryId: webDev.Id,
            title: "Clean Architecture with .NET 8",
            description: "Learn how to structure real-world .NET applications using Clean Architecture, CQRS, MediatR, and Domain-Driven Design principles.",
            price: 59.99m,
            level: CourseLevel.Advanced,
            language: "English",
            thumbnailUrl:"nature.png");

        course2.Publish();
        await context.Courses.AddAsync(course2);
        await context.SaveChangesAsync();

        await SeedCourse2Sections(context, course2.Id);

        // ── Course 3: Python Machine Learning (Published) ──────────
        var course3 = Course.Create(
            instructorId: sarah.Id,
            categoryId: ml.Id,
            title: "Machine Learning with Python — Hands On",
            description: "From linear regression to neural networks. Build real ML models using Python, scikit-learn, and TensorFlow.",
            price: 69.99m,
            level: CourseLevel.Beginner,
            language: "English",
            thumbnailUrl: "ml.png");

        course3.Publish();
        await context.Courses.AddAsync(course3);
        await context.SaveChangesAsync();

        await SeedCourse3Sections(context, course3.Id);

        // ── Course 4: React for Beginners (Published, Free) ────────
        var course4 = Course.Create(
            instructorId: sarah.Id,
            categoryId: webDev.Id,
            title: "React.js for Beginners",
            description: "Learn React from zero. Components, hooks, state management, and building your first full React application.",
            price: 0m,
            level: CourseLevel.Beginner,
            language: "English",
            thumbnailUrl:"ai.png");

        course4.Publish();
        await context.Courses.AddAsync(course4);
        await context.SaveChangesAsync();

        await SeedCourse4Sections(context, course4.Id);

        // ── Course 5: Mobile with Flutter (Draft) ──────────────────
        var course5 = Course.Create(
            instructorId: mohamed.Id,
            categoryId: mobile.Id,
            title: "Flutter & Dart — Mobile Development",
            description: "Build beautiful cross-platform mobile apps with Flutter and Dart. iOS and Android from a single codebase.",
            price: 44.99m,
            level: CourseLevel.Intermediate,
            language: "Arabic",
            thumbnailUrl: "flutter.png");

        // Left as Draft intentionally — to test enrollment rejection
        await context.Courses.AddAsync(course5);
        await context.SaveChangesAsync();

        await SeedCourse5Sections(context, course5.Id);

        logger.LogInformation("Seeded 5 courses (4 published, 1 draft).");
    }

    // ── Course 1 Sections ──────────────────────────────────────────

    private static async Task SeedCourse1Sections(LMSDbContext context, Guid courseId)
    {
        var sec1 = Section.Create(courseId, "Getting Started", 0);
        var sec2 = Section.Create(courseId, "Building Your First API", 1);
        var sec3 = Section.Create(courseId, "Authentication & Authorization", 2);
        var sec4 = Section.Create(courseId, "Entity Framework Core", 3);

        await context.Sections.AddRangeAsync(sec1, sec2, sec3, sec4);
        await context.SaveChangesAsync();

        var lessons = new List<Lesson>
        {
            // Section 1
            Lesson.Create(sec1.Id, "Course Overview",              "https://storage.lms.com/videos/c1s1l1.mp4", ContentType.Video,   300,  0, isFreePreview: true),
            Lesson.Create(sec1.Id, "Setting Up the Environment",   "https://storage.lms.com/videos/c1s1l2.mp4", ContentType.Video,   480,  1, isFreePreview: true),
            Lesson.Create(sec1.Id, "Project Structure Explained",  "https://storage.lms.com/docs/c1s1l3.pdf",   ContentType.PDF,     180,  2),

            // Section 2
            Lesson.Create(sec2.Id, "Creating Controllers",         "https://storage.lms.com/videos/c1s2l1.mp4", ContentType.Video,   720,  0),
            Lesson.Create(sec2.Id, "Action Methods & Routing",     "https://storage.lms.com/videos/c1s2l2.mp4", ContentType.Video,   600,  1),
            Lesson.Create(sec2.Id, "Request & Response Models",    "https://storage.lms.com/videos/c1s2l3.mp4", ContentType.Video,   540,  2),
            Lesson.Create(sec2.Id, "HTTP Status Codes Guide",      "https://storage.lms.com/docs/c1s2l4.pdf",   ContentType.PDF,     120,  3),

            // Section 3
            Lesson.Create(sec3.Id, "JWT Authentication Setup",     "https://storage.lms.com/videos/c1s3l1.mp4", ContentType.Video,   900,  0),
            Lesson.Create(sec3.Id, "Refresh Token Implementation", "https://storage.lms.com/videos/c1s3l2.mp4", ContentType.Video,   780,  1),
            Lesson.Create(sec3.Id, "Role-Based Authorization",     "https://storage.lms.com/videos/c1s3l3.mp4", ContentType.Video,   660,  2),

            // Section 4
            Lesson.Create(sec4.Id, "EF Core Setup & DbContext",    "https://storage.lms.com/videos/c1s4l1.mp4", ContentType.Video,   720,  0),
            Lesson.Create(sec4.Id, "Code-First Migrations",        "https://storage.lms.com/videos/c1s4l2.mp4", ContentType.Video,   600,  1),
            Lesson.Create(sec4.Id, "Repository Pattern",           "https://storage.lms.com/videos/c1s4l3.mp4", ContentType.Video,   840,  2),
        };

        await context.Lessons.AddRangeAsync(lessons);
        await context.SaveChangesAsync();
    }

    // ── Course 2 Sections ──────────────────────────────────────────

    private static async Task SeedCourse2Sections(LMSDbContext context, Guid courseId)
    {
        var sec1 = Section.Create(courseId, "Clean Architecture Fundamentals", 0);
        var sec2 = Section.Create(courseId, "Domain Layer", 1);
        var sec3 = Section.Create(courseId, "Application Layer & CQRS", 2);
        var sec4 = Section.Create(courseId, "Infrastructure Layer", 3);

        await context.Sections.AddRangeAsync(sec1, sec2, sec3, sec4);
        await context.SaveChangesAsync();

        var lessons = new List<Lesson>
        {
            Lesson.Create(sec1.Id, "What is Clean Architecture?",       "https://storage.lms.com/videos/c2s1l1.mp4", ContentType.Video, 540,  0, isFreePreview: true),
            Lesson.Create(sec1.Id, "Dependency Rule Explained",         "https://storage.lms.com/videos/c2s1l2.mp4", ContentType.Video, 480,  1, isFreePreview: true),
            Lesson.Create(sec1.Id, "Setting Up the Solution",          "https://storage.lms.com/videos/c2s1l3.mp4", ContentType.Video, 600,  2),

            Lesson.Create(sec2.Id, "Entities & Value Objects",          "https://storage.lms.com/videos/c2s2l1.mp4", ContentType.Video, 720,  0),
            Lesson.Create(sec2.Id, "Domain Events",                     "https://storage.lms.com/videos/c2s2l2.mp4", ContentType.Video, 660,  1),
            Lesson.Create(sec2.Id, "Domain Errors Pattern",             "https://storage.lms.com/videos/c2s2l3.mp4", ContentType.Video, 480,  2),

            Lesson.Create(sec3.Id, "CQRS with MediatR",                 "https://storage.lms.com/videos/c2s3l1.mp4", ContentType.Video, 900,  0),
            Lesson.Create(sec3.Id, "Commands & Handlers",               "https://storage.lms.com/videos/c2s3l2.mp4", ContentType.Video, 780,  1),
            Lesson.Create(sec3.Id, "FluentValidation Pipeline",         "https://storage.lms.com/videos/c2s3l3.mp4", ContentType.Video, 600,  2),

            Lesson.Create(sec4.Id, "EF Core Configurations",            "https://storage.lms.com/videos/c2s4l1.mp4", ContentType.Video, 720,  0),
            Lesson.Create(sec4.Id, "Unit of Work Pattern",              "https://storage.lms.com/videos/c2s4l2.mp4", ContentType.Video, 660,  1),
        };

        await context.Lessons.AddRangeAsync(lessons);
        await context.SaveChangesAsync();
    }

    // ── Course 3 Sections ──────────────────────────────────────────

    private static async Task SeedCourse3Sections(LMSDbContext context, Guid courseId)
    {
        var sec1 = Section.Create(courseId, "Python Fundamentals", 0);
        var sec2 = Section.Create(courseId, "Data Processing with Pandas", 1);
        var sec3 = Section.Create(courseId, "Machine Learning Models", 2);

        await context.Sections.AddRangeAsync(sec1, sec2, sec3);
        await context.SaveChangesAsync();

        var lessons = new List<Lesson>
        {
            Lesson.Create(sec1.Id, "Python Setup & Basics",             "https://storage.lms.com/videos/c3s1l1.mp4", ContentType.Video, 420,  0, isFreePreview: true),
            Lesson.Create(sec1.Id, "Data Types & Control Flow",         "https://storage.lms.com/videos/c3s1l2.mp4", ContentType.Video, 540,  1),
            Lesson.Create(sec1.Id, "Functions & Modules",               "https://storage.lms.com/videos/c3s1l3.mp4", ContentType.Video, 480,  2),

            Lesson.Create(sec2.Id, "Introduction to Pandas",            "https://storage.lms.com/videos/c3s2l1.mp4", ContentType.Video, 660,  0),
            Lesson.Create(sec2.Id, "DataFrames & Filtering",            "https://storage.lms.com/videos/c3s2l2.mp4", ContentType.Video, 720,  1),
            Lesson.Create(sec2.Id, "Data Cleaning Techniques",          "https://storage.lms.com/videos/c3s2l3.mp4", ContentType.Video, 600,  2),

            Lesson.Create(sec3.Id, "Linear Regression",                 "https://storage.lms.com/videos/c3s3l1.mp4", ContentType.Video, 840,  0),
            Lesson.Create(sec3.Id, "Classification Algorithms",         "https://storage.lms.com/videos/c3s3l2.mp4", ContentType.Video, 900,  1),
            Lesson.Create(sec3.Id, "Model Evaluation & Tuning",         "https://storage.lms.com/videos/c3s3l3.mp4", ContentType.Video, 780,  2),
        };

        await context.Lessons.AddRangeAsync(lessons);
        await context.SaveChangesAsync();
    }

    // ── Course 4 Sections ──────────────────────────────────────────

    private static async Task SeedCourse4Sections(LMSDbContext context, Guid courseId)
    {
        var sec1 = Section.Create(courseId, "React Basics", 0);
        var sec2 = Section.Create(courseId, "Hooks & State Management", 1);
        var sec3 = Section.Create(courseId, "Building a Real App", 2);

        await context.Sections.AddRangeAsync(sec1, sec2, sec3);
        await context.SaveChangesAsync();

        var lessons = new List<Lesson>
        {
            Lesson.Create(sec1.Id, "What is React?",                    "https://storage.lms.com/videos/c4s1l1.mp4", ContentType.Video, 360,  0, isFreePreview: true),
            Lesson.Create(sec1.Id, "JSX & Components",                  "https://storage.lms.com/videos/c4s1l2.mp4", ContentType.Video, 480,  1, isFreePreview: true),
            Lesson.Create(sec1.Id, "Props & Component Composition",     "https://storage.lms.com/videos/c4s1l3.mp4", ContentType.Video, 540,  2),

            Lesson.Create(sec2.Id, "useState Hook",                     "https://storage.lms.com/videos/c4s2l1.mp4", ContentType.Video, 600,  0),
            Lesson.Create(sec2.Id, "useEffect Hook",                    "https://storage.lms.com/videos/c4s2l2.mp4", ContentType.Video, 660,  1),
            Lesson.Create(sec2.Id, "Custom Hooks",                      "https://storage.lms.com/videos/c4s2l3.mp4", ContentType.Video, 540,  2),

            Lesson.Create(sec3.Id, "Project Setup & Planning",          "https://storage.lms.com/videos/c4s3l1.mp4", ContentType.Video, 420,  0),
            Lesson.Create(sec3.Id, "Fetching Data from an API",         "https://storage.lms.com/videos/c4s3l2.mp4", ContentType.Video, 720,  1),
            Lesson.Create(sec3.Id, "Deploying Your React App",          "https://storage.lms.com/videos/c4s3l3.mp4", ContentType.Video, 480,  2),
        };

        await context.Lessons.AddRangeAsync(lessons);
        await context.SaveChangesAsync();
    }

    // ── Course 5 Sections (Draft) ──────────────────────────────────

    private static async Task SeedCourse5Sections(LMSDbContext context, Guid courseId)
    {
        var sec1 = Section.Create(courseId, "Dart Language Basics", 0);
        var sec2 = Section.Create(courseId, "Flutter Widgets", 1);

        await context.Sections.AddRangeAsync(sec1, sec2);
        await context.SaveChangesAsync();

        var lessons = new List<Lesson>
        {
            Lesson.Create(sec1.Id, "Introduction to Dart",              "https://storage.lms.com/videos/c5s1l1.mp4", ContentType.Video, 420,  0, isFreePreview: true),
            Lesson.Create(sec1.Id, "Variables & Functions in Dart",     "https://storage.lms.com/videos/c5s1l2.mp4", ContentType.Video, 540,  1),

            Lesson.Create(sec2.Id, "Stateless vs Stateful Widgets",     "https://storage.lms.com/videos/c5s2l1.mp4", ContentType.Video, 600,  0),
            Lesson.Create(sec2.Id, "Layouts & Navigation",              "https://storage.lms.com/videos/c5s2l2.mp4", ContentType.Video, 660,  1),
        };

        await context.Lessons.AddRangeAsync(lessons);
        await context.SaveChangesAsync();
    }

    // QuizSeeding
    private static async Task SeedQuizzesAsync(LMSDbContext context, ILogger logger)
    {
        if (await context.Quizzes.AnyAsync())
        {
            logger.LogInformation("Quizzes already seeded — skipping.");
            return;

        }

        // Load courses created in DatabaseSeeder
        var course1 = await context.Courses.FirstAsync(c => c.Title.Contains("ASP.NET Core"));
        var course2 = await context.Courses.FirstAsync(c => c.Title.Contains("Clean Architecture"));
        var course3 = await context.Courses.FirstAsync(c => c.Title.Contains("Machine Learning"));

        // ── Quiz 1: ASP.NET Core Basics (PassScore = 70) ─────────────────────────
        // Scenario: Student answers everything correct → Score 100, Passed
        // Scenario: Student answers 2/4 correct     → Score 50,  Failed
        // Scenario: Student answers 3/4 correct     → Score 75,  Passed

        var quiz1 = Quiz.Create(
            courseId: course1.Id,
            title: "ASP.NET Core Fundamentals Quiz",
            passScore: 70,
            timeLimitMinutes: 20);

        // Q1 — SingleChoice (Points = 1)
        // Correct: B (HTTP is stateless)
        var q1 = Question.Create(quiz1.Id, "What does HTTP stand for?", QuestionType.SingleChoice, points: 1);
        q1.AddAnswers(new[]
        {
            ("HyperText Transfer Protocol",  true),   // ← correct
            ("High Transfer Text Protocol",  false),
            ("HyperText Transmission Path",  false),
            ("Hyper Transfer Text Process",  false),
        });

        // Q2 — SingleChoice (Points = 1)
        // Correct: C (200 = OK)
        var q2 = Question.Create(quiz1.Id, "Which HTTP status code means the request was successful?", QuestionType.SingleChoice, points: 1);
        q2.AddAnswers(new[]
        {
            ("301", false),
            ("404", false),
            ("200", true),    // ← correct
            ("500", false),
        });

        // Q3 — SingleChoice (Points = 2)
        // Higher points — worth more in score calculation
        var q3 = Question.Create(quiz1.Id, "Which HTTP method is used to UPDATE an existing resource?", QuestionType.SingleChoice, points: 2);
        q3.AddAnswers(new[]
        {
            ("GET",    false),
            ("POST",   false),
            ("DELETE", false),
            ("PUT",    true),    // ← correct
        });

        // Q4 — MultiChoice (Points = 2)
        // Student must select BOTH correct answers — all-or-nothing
        var q4 = Question.Create(quiz1.Id, "Which of the following are valid HTTP methods? (Select all that apply)", QuestionType.MultiChoice, points: 2);
        q4.AddAnswers(new[]
        {
            ("GET",    true),    // ← correct
            ("FETCH",  false),
            ("POST",   true),    // ← correct
            ("SEND",   false),
        });

        quiz1.Questions.Add(q1);  // This adds q1 to the in-memory collection
        quiz1.Questions.Add(q2);
        quiz1.Questions.Add(q3);
        quiz1.Questions.Add(q4);

        await context.Quizzes.AddAsync(quiz1);
        await context.SaveChangesAsync();  // Everything saves together

       

        // ── Score scenarios for Quiz 1 ────────────────────────────────────────────
        // totalPoints = 1 + 1 + 2 + 2 = 6
        //
        // Student gets ALL correct:
        //   earnedPoints = 6 → score = 6/6 × 100 = 100 → Passed ✅
        //
        // Student gets Q1 + Q2 only:
        //   earnedPoints = 1 + 1 = 2 → score = 2/6 × 100 = 33 → Failed ❌
        //
        // Student gets Q1 + Q2 + Q3:
        //   earnedPoints = 1 + 1 + 2 = 4 → score = 4/6 × 100 = 67 → Failed ❌ (just under 70)
        //
        // Student gets Q1 + Q3 + Q4:
        //   earnedPoints = 1 + 2 + 2 = 5 → score = 5/6 × 100 = 83 → Passed ✅


        // ── Quiz 2: Clean Architecture (PassScore = 80, harder) ──────────────────

        var quiz2 = Quiz.Create(
            courseId: course2.Id,
            title: "Clean Architecture Concepts Quiz",
            passScore: 80,
            timeLimitMinutes: 15);

        // Q1 — TrueFalse (Points = 1)
        var q5 = Question.Create(quiz2.Id, "In Clean Architecture, the Domain layer can depend on the Infrastructure layer.", QuestionType.TrueFalse, points: 1);
        q5.AddAnswers(new[]
        {
            ("True",  false),
            ("False", true),    // ← correct — Domain has NO dependencies
        });

        // Q2 — SingleChoice (Points = 2)
        var q6 = Question.Create(quiz2.Id, "Which pattern does MediatR implement?", QuestionType.SingleChoice, points: 2);
        q6.AddAnswers(new[]
        {
            ("Repository",  false),
            ("Mediator",    true),    // ← correct
            ("Observer",    false),
            ("Singleton",   false),
        });

        // Q3 — SingleChoice (Points = 2)
        var q7 = Question.Create(quiz2.Id, "In CQRS, what does a Command do?", QuestionType.SingleChoice, points: 2);
        q7.AddAnswers(new[]
        {
            ("Reads data from the database",       false),
            ("Changes state / writes data",        true),    // ← correct
            ("Handles authentication",             false),
            ("Maps objects to database tables",    false),
        });

        // Q4 — MultiChoice (Points = 3)
        // Must select ALL 3 correct options
        var q8 = Question.Create(quiz2.Id, "Which of these belong to the Application layer? (Select all that apply)", QuestionType.MultiChoice, points: 3);
        q8.AddAnswers(new[]
        {
            ("Commands",          true),     // ← correct
            ("DbContext",         false),
            ("Queries",           true),     // ← correct
            ("DTOs",              true),     // ← correct
            ("EF Core Configs",   false),
        });

        quiz2.Questions.Add(q5);
        quiz2.Questions.Add(q6);
        quiz2.Questions.Add(q7);
        quiz2.Questions.Add(q8);

        await context.Quizzes.AddAsync(quiz2);
        await context.SaveChangesAsync();

        // ── Score scenarios for Quiz 2 ────────────────────────────────────────────
        // totalPoints = 1 + 2 + 2 + 3 = 8
        //
        // Student gets ALL correct:
        //   earnedPoints = 8 → score = 100 → Passed ✅
        //
        // Student gets Q1 + Q2 + Q3 (misses multi-choice):
        //   earnedPoints = 1 + 2 + 2 = 5 → score = 5/8 × 100 = 63 → Failed ❌
        //
        // Student gets Q2 + Q3 + Q4:
        //   earnedPoints = 2 + 2 + 3 = 7 → score = 7/8 × 100 = 88 → Passed ✅
        //
        // Student gets only Q4 (multi-choice, highest points):
        //   earnedPoints = 3 → score = 3/8 × 100 = 38 → Failed ❌


        // ── Quiz 3: Python ML Basics (PassScore = 60, easier) ────────────────────

        var quiz3 = Quiz.Create(
            courseId: course3.Id,
            title: "Machine Learning Basics Quiz",
            passScore: 60,
            timeLimitMinutes: 30);

        // Q1 — TrueFalse (Points = 1)
        var q9 = Question.Create(quiz3.Id, "Supervised learning requires labeled training data.", QuestionType.TrueFalse, points: 1);
        q9.AddAnswers(new[]
        {
            ("True",  true),    // ← correct
            ("False", false),
        });

        // Q2 — SingleChoice (Points = 1)
        var q10 = Question.Create(quiz3.Id, "Which Python library is commonly used for data manipulation?", QuestionType.SingleChoice, points: 1);
        q10.AddAnswers(new[]
        {
            ("NumPy",      false),
            ("Pandas",     true),    // ← correct
            ("Matplotlib", false),
            ("Flask",      false),
        });

        // Q3 — SingleChoice (Points = 2)
        var q11 = Question.Create(quiz3.Id, "What is overfitting in Machine Learning?", QuestionType.SingleChoice, points: 2);
        q11.AddAnswers(new[]
        {
            ("Model performs well on training data but poorly on new data", true),   // ← correct
            ("Model performs poorly on both training and test data",        false),
            ("Model is too simple to capture patterns",                    false),
            ("Model has too few parameters",                               false),
        });

        // Q4 — MultiChoice (Points = 2)
        var q12 = Question.Create(quiz3.Id, "Which of these are supervised learning algorithms? (Select all that apply)", QuestionType.MultiChoice, points: 2);
        q12.AddAnswers(new[]
        {
            ("Linear Regression",   true),    // ← correct
            ("K-Means Clustering",  false),
            ("Decision Tree",       true),    // ← correct
            ("PCA",                 false),
        });

        // Q5 — SingleChoice (Points = 2)
        var q13 = Question.Create(quiz3.Id, "What does a confusion matrix measure?", QuestionType.SingleChoice, points: 2);
        q13.AddAnswers(new[]
        {
            ("Model training speed",              false),
            ("Classification model performance",  true),    // ← correct
            ("Data preprocessing quality",        false),
            ("Feature importance",                false),
        });

        quiz3.Questions.Add(q9);
        quiz3.Questions.Add(q10);
        quiz3.Questions.Add(q11);
        quiz3.Questions.Add(q12);
        quiz3.Questions.Add(q13);

        await context.Quizzes.AddAsync(quiz3);
        await context.SaveChangesAsync();
        // ── Score scenarios for Quiz 3 ────────────────────────────────────────────
        // totalPoints = 1 + 1 + 2 + 2 + 2 = 8
        //
        // Student gets ALL correct:
        //   earnedPoints = 8 → score = 100 → Passed ✅
        //
        // Student gets Q1 + Q2 only:
        //   earnedPoints = 1 + 1 = 2 → score = 25 → Failed ❌
        //
        // Student gets Q1 + Q2 + Q3:
        //   earnedPoints = 1 + 1 + 2 = 4 → score = 50 → Failed ❌ (just under 60)
        //
        // Student gets Q1 + Q2 + Q3 + Q5:
        //   earnedPoints = 1 + 1 + 2 + 2 = 6 → score = 75 → Passed ✅
        //
        // Student selects only ONE answer in Q4 (MultiChoice needs BOTH):
        //   earnedPoints for Q4 = 0 → all-or-nothing


        logger.LogInformation("Seeded 3 quizzes with 12 questions total.");
    }
    // ── Helper ─────────────────────────────────────────────────────

    private static User CreateUser(
        string fullName,
        string email,
        string password,
        UserRole role,
        string? phoneNumber = null)
    {
        var user = User.CreateLocal(
            fullName: fullName,
            email: email,
            passwordHash: BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12),
            phoneNumber: phoneNumber);

        user.ChangeRole(role);
        return user;
    }
}