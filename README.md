# LMS.API

A Learning Management System backend built on **ASP.NET Core 10** following **Clean Architecture** and the **CQRS** pattern with **MediatR**, **Entity Framework Core** (SQL Server), **JWT** authentication, **Google** sign-in, and structured **Serilog** logging.

---

## Table of Contents

- [Architecture](#architecture)
- [Solution Layout](#solution-layout)
- [Request Workflow](#request-workflow)
- [Domain Model](#domain-model)
- [Modules / Features](#modules--features)
- [Cross-Cutting Concerns](#cross-cutting-concerns)
- [Tech Stack](#tech-stack)
- [Prerequisites](#prerequisites)
- [Configuration](#configuration)
- [Getting Started](#getting-started)
- [Database Migrations & Seeding](#database-migrations--seeding)
- [Testing](#testing)
- [Logging & Observability](#logging--observability)
- [Authorization Policies](#authorization-policies)
- [Roadmap](#roadmap)

---

## Architecture

The solution is split into four projects that follow the **Dependency Rule**: source code dependencies point only inward.

```
                ┌────────────────────────────────────────────┐
                │                LMS.API                     │  ← Presentation
                │  Controllers · Middleware · Swagger · DI  │
                └────────────┬───────────────────────────────┘
                             │ references
                ┌────────────▼───────────────┐
                │       LMS.Application      │  ← Application
                │ Commands/Queries (MediatR) │  Validation (FluentValidation)
                │ DTOs · Interfaces · Behaviors
                └────────────┬───────────────┘
                             │ references
                ┌────────────▼───────────────┐
                │         LMS.Domain         │  ← Enterprise
                │ Entities · Value Objects   │  Enums · Errors · Domain Events
                │ Repository Interfaces · IUnitOfWork
                └────────────▲───────────────┘
                             │ implemented by
                ┌────────────┴───────────────┐
                │     LMS.Infrastructure     │  ← Infrastructure
                │ EF Core · SQL Server       │  Repositories · JwtService
                │ GoogleAuth · Migrations · Seeders
                └────────────────────────────┘
```

Key principles applied:

- **Domain has no dependencies** — entities, value objects, enums, errors, domain events, and repository *interfaces* live there.
- **Application depends only on Domain** — use cases (Commands/Queries) and DTOs; it owns no infrastructure concerns.
- **Infrastructure implements Application/Domain abstractions** — EF Core, password hashing, JWT, Google auth.
- **API is a thin host** — controllers translate HTTP into MediatR requests; they never call repositories directly.
- **CQRS via MediatR** — every operation is either a `Command` (state-changing) or a `Query` (read-only), each with its own `Handler`.
- **Domain Events** implement `INotification` (MediatR) and are dispatched by handlers in the same unit of work (e.g. `EnrollmentCompletedEvent` → certificate issuance).

---

## Solution Layout

```
LMS.API.slnx
├── LMS.API/                          ← ASP.NET Core Web API host
│   ├── Controllers/                  ← Thin HTTP → MediatR adapters (17)
│   ├── Middleware/                   ← GlobalExceptionHandler, RequestLogging
│   ├── Logging/                      ← InMemorySink, ShortSourceContextEnricher
│   ├── Extensions/                   ← LoggingExtensions, CurrentUserService
│   ├── Program.cs                    ← Composition root
│   └── appsettings*.json
│
├── LMS.Application/                  ← Use cases & contracts
│   ├── Common/
│   │   ├── Behaviors/ValidationBehavior.cs
│   │   ├── Interfaces/  (ICurrentUserService, IJwtService, IPasswordHasher,
│   │   │                IGoogleAuthService, IDatabaseSeeder)
│   │   ├── Models/      (Result, AuthResponse)
│   │   └── Settings/    (JwtSettings)
│   ├── DependencyInjection/ApplicationServiceExtensions.cs
│   ├── DTOs/            (per-feature DTOs)
│   └── Features/        ← one folder per bounded context
│       ├── Admin/       · Auth/         · Category/
│       ├── Courses/     · Enrollments/  · Lessons/
│       ├── LessonProgress/ · Notifications/ · Progress/
│       ├── Question/    · Quiz/         · Reviews/
│       ├── Section/     · Toggle/       · Users/
│       ├── Attemps/     · Answer/       · Certificates/
│
├── LMS.Domain/                        ← Pure domain model
│   ├── Primitives/    (Entity, AuditableEntity, IDomainEvent)
│   ├── Entities/      (User, Course, Section, Lesson, Enrollment,
│   │                  Quiz, Question, Answer, QuizAttempt,
│   │                  QuizAttemptAnswer, LessonProgress, Certificate,
│   │                  Category, Tag, CourseTag, Review, Notification,
│   │                  RefreshToken)
│   ├── Enums/         (UserRole, AuthProvider, CourseLevel, CourseStatus,
│   │                  EnrollmentStatus, QuestionType, ReviewTargetType,
│   │                  ContentType)
│   ├── Errors/        (Error, DomainErrors)
│   ├── Events/        (UserRegisteredEvent, CoursePublishedEvent,
│   │                  EnrollmentCompletedEvent)
│   └── Interfaces/    (IRepository<>, IUnitOfWork,
│                       Interfaces/Repositories/* per aggregate)
│
├── LMS.Infrastructure/               ← EF Core, auth, external services
│   ├── DependencyInjection/InfrastructureServiceExtensions.cs
│   ├── Persistence/
│   │   ├── LMSDbContext.cs
│   │   ├── DesignTimeDbContextFactory.cs
│   │   ├── Repositories/   (concrete impls per aggregate + UnitOfWork)
│   │   ├── Seeders/        (DatabaseSeeder, SeederExtensions)
│   │   └── Migrations/     (EF Core migrations)
│   └── Services/Auth/     (JwtService, PasswordHasher, GoogleAuthService)
│
└── LMS.Application.UnitTests/         ← MSTest + Moq (handler-level tests)
    ├── Controllers/
    └── Features/...                  (handler + event-handler tests)
```

---

## Request Workflow

A typical request flows through the system as follows:

```
HTTP Client
   │  Bearer JWT
   ▼
┌──────────────────────────────────────────────────────────────┐
│ Kestrel → ASP.NET Core pipeline                              │
│   1. UseExceptionHandler()      (RFC 7807 ProblemDetails)    │
│   2. UseHttpsRedirection()                                    │
│   3. UseCors("LmsPolicy")                                    │
│   4. UseAuthentication()       (JwtBearer)                   │
│   5. RequestLoggingMiddleware  (correlation id + per-req log)│
│   6. UseAuthorization()                                      │
│   7. MapControllers()                                         │
└──────────────────────────────────────────────────────────────┘
   │
   ▼
Controller (e.g. AuthController, CourseController)
   │  Deserializes body into a Command/Query DTO
   │  Calls MediatR: _mediator.Send(new LoginCommand(...))
   ▼
MediatR pipeline
   │  ► ValidationBehavior<TReq,TResp>     (FluentValidation)
   │  ► (future) LoggingBehavior, etc.
   ▼
CommandHandler / QueryHandler    (Application layer)
   │  Resolves dependencies via DI:
   │    ICurrentUserService · IUserRepository · IUnitOfWork
   │    IJwtService · IPasswordHasher · IGoogleAuthService
   │  Executes domain logic, raises domain events
   ▼
Infrastructure
   │  EF Core → LMSDbContext → SQL Server
   │  Repositories + UnitOfWork persist changes
   ▼
Result<T>  (success | failure(Error))
   │
   ▼
Controller maps to IActionResult
   │  Success  → 200/201 + DTO
   │  Failure  → ProblemDetails (status from Error code)
   ▼
HTTP Response
```

**Domain events** (e.g. `UserRegisteredEvent`, `EnrollmentCompletedEvent`) are raised inside aggregates, collected in `Entity.DomainEvents`, and published via MediatR after `SaveChangesAsync`. Sample flow: an enrollment completes → `EnrollmentCompletedEvent` is dispatched → `EnrollmentCompletedEventHandler` issues a `Certificate` for the student.

---

## Domain Model

Aggregates & key invariants (all entities inherit `AuditableEntity` → `Entity` with `Id`, `CreatedAt`, `UpdatedAt`, and `DomainEvents`):

| Aggregate | Purpose |
|-----------|---------|
| **User** | Local + Google accounts, role (Student / Instructor / Admin / SuperAdmin), active flag, factory methods emit `UserRegisteredEvent`. |
| **RefreshToken** | Rotating JWT refresh tokens tied to a user. |
| **Course → Section → Lesson** | Hierarchical content tree. Lessons carry `ContentType` (Video/PDF/...). |
| **Category / Tag / CourseTag** | Course taxonomy; `CourseTag` is a composite-key join. |
| **Enrollment** | Student ⇄ Course; unique per pair; carries status + paid price. |
| **LessonProgress** | Per-lesson completion tracking; unique per (Enrollment, Lesson). |
| **Certificate** | Issued when an enrollment completes (1:1 with Enrollment). |
| **Quiz → Question → Answer** | Quizzes contain questions with multiple-choice answers. |
| **QuizAttempt → QuizAttemptAnswer** | Student attempts; answers are graded into score + per-question selection. |
| **Review** | Reviews against Courses *or* Instructors via `ReviewTargetType`. |
| **Notification** | In-app notifications per user. |

Concurrency: `DbContext.SaveChangesAsync` auto-stamps `CreatedAt` / `UpdatedAt`. Cascade cycles are explicitly disabled (`NoAction`) on most relationships; `Question → Quiz` and `Answer → Question` cascade-delete.

---

## Modules / Features

Each module under `LMS.Application/Features/*` follows a consistent shape:

```
/Features/<BoundedContext>/
   ├── Commands/<Action>/<Action>Command.cs
   ├── Commands/<Action>/<Action>CommandHandler.cs
   ├── Commands/<Action>/<Action>CommandValidator.cs   (when needed)
   └── Queries/<Query>/<Query>.cs + QueryHandler.cs
```

Controllers are 1‑to‑1 with bounded contexts (see `LMS.API/Controllers/`):

- **AuthController** — register, login, refresh, logout, Google sign-in
- **UsersController** — profile, change password, admin: list users
- **AdminController** — activate/deactivate users, change role, search instructors
- **CategoryController / CourseController / SectionController / LessonController**
- **EnrollmentsController / ProgressController / CertificatesController**
- **QuizController / QuestionController / AnswerController / QuizAttemptController**
- **ReviewsController / NotificationsController**
- **LogsController** — admin-only view of the in-memory Serilog buffer

---

## Cross-Cutting Concerns

- **Validation** — `ValidationBehavior<TRequest, TResponse>` is registered as a MediatR pipeline behavior; validators live next to their command (FluentValidation).
- **Result pattern** — handlers return `Result` / `Result<T>` (`LMS.Application/Common/Models/Result.cs`) carrying an `Error` (`Code` + `Description`). Controllers map failures to `ProblemDetails` with an appropriate HTTP status.
- **Current user** — `ICurrentUserService` is resolved per-request from `IHttpContextAccessor` (impl: `CurrentUserService`).
- **Exceptions** — `GlobalExceptionHandler` (registered via `AddExceptionHandler<T>()`) maps `ValidationException`, `UnauthorizedAccessException`, `KeyNotFoundException`, `InvalidOperationException`, and unknown errors to RFC 7807 responses with `traceId` and `timestamp`.
- **Correlation IDs** — `RequestLoggingMiddleware` reads/injects `X-Correlation-ID`, pushes it into Serilog's `LogContext`, echoes it on the response.
- **CORS** — origins configured under `Cors:AllowedOrigins`, policy `LmsPolicy`.
- **OpenAPI** — Swashbuckle with XML comments and a Bearer JWT security scheme; UI mounted at `/`.

---

## Tech Stack

| Area | Choice |
|------|--------|
| Runtime | .NET 10 (`net10.0`) |
| Web | ASP.NET Core Web API, SignalR |
| Mediator | MediatR 12 |
| Validation | FluentValidation 12 |
| ORM | EF Core 10 + EF Core SqlServer |
| Database | SQL Server (LocalDB or hosted) |
| AuthN/AuthZ | ASP.NET Core JWT Bearer + role-based policies |
| Identity | BCrypt for password hashing, Google OAuth (`Google.Apis.Auth`) |
| Logging | Serilog 4 → console + rolling files (text & JSON) + in-memory ring buffer |
| Docs | Swashbuckle / Swagger UI |
| Tests | MSTest 4 + Moq |

---

## Prerequisites

- [.NET SDK 10](https://dotnet.microsoft.com/)
- SQL Server — LocalDB (default in `appsettings.json` is commented) **or** any reachable SQL Server instance. The committed `DefaultConnection` points to a hosted database (`db53550.public.databaseasp.net`) — replace it for local development.
- A Google OAuth **Client ID** if you intend to test `POST /api/auth/google`.

---

## Configuration

`LMS.API/appsettings.json` (override with `appsettings.Development.json` or environment variables):

```jsonc
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=...;Database=LMSDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True"
  },
  "Jwt": {
    "Secret": "<at-least-32-chars>",
    "Issuer": "LMS.API",
    "Audience": "LMS.Client",
    "ExpiryMinutes": "60",
    "RefreshTokenExpiryDays": "7"
  },
  "Google": { "ClientId": "<google-oauth-client-id>" },
  "Cors":  { "AllowedOrigins": [ "https://your-frontend.example" ] }
}
```

> ⚠️ **Never commit real secrets.** Override `Jwt:Secret` and `Google:ClientId` via environment variables, `dotnet user-secrets`, or your secret manager of choice.

---

## Getting Started

```powershell
# from the repository root
dotnet restore

# run the API (it will pick up appsettings.Development.json)
dotnet run --project LMS.API

# browse Swagger UI
# https://localhost:<port>/  (root)
```

On startup in **Development** the host automatically:

1. Applies pending EF Core migrations.
2. Runs the database seeder (`LMS.Infrastructure/Persistence/Seeders/DatabaseSeeder.cs`).

In any other environment, migrations are **not** applied automatically — run them explicitly (see below).

---

## Database Migrations & Seeding

The migration files live under `LMS.Infrastructure/Migrations`. Use the EF Core CLI from the API project:

```powershell
# add a new migration after a model change
dotnet ef migrations add <Name> \
  --project LMS.Infrastructure \
  --startup-project LMS.API

# apply migrations
dotnet ef database update \
  --project LMS.Infrastructure \
  --startup-project LMS.API

# generate SQL script (for production deploys)
dotnet ef migrations script \
  --project LMS.Infrastructure \
  --startup-project LMS.API \
  --idempotent -o migrate.sql
```

`IDatabaseSeeder` is run once at startup in Development to populate reference data (categories, demo users, etc.). Add new seed entries by extending `DatabaseSeeder.cs`.

---

## Testing

`LMS.Application.UnitTests` is an MSTest + Moq project that exercises handlers, event handlers, and controller authorization in isolation. EF Core repositories and external services are mocked.

```powershell
dotnet test LMS.Application.UnitTests
```

Existing test surfaces (representative — extend as needed):

- `Features/Enrollments/Commands/EnrollStudent/EnrollStudentCommandHandlerTests.cs`
- `Features/LessonProgress/Commands/UpdateLessonProgressCommandHandlerTests.cs`
- `Features/Certificates/EventHandlers/EnrollmentCompletedEventHandlerTests.cs`
- `Features/Users/Queries/GetAllUsers/GetAllUsersQueryHandlerTests.cs`
- `Controllers/EnrollmentsControllerAuthorizationTests.cs`

---

## Logging & Observability

Serilog is wired in `Extensions/LoggingExtensions.cs#AddSerilogLogging` with three sinks:

1. **Console** — colored output for local dev.
2. **Rolling files** — `logs/LMS.API-<env>/logs-<yyyyMMdd>.{txt,json}` (10 MB cap, 30 retained).
3. **In-memory ring buffer** — `Logging/InMemorySink.cs` keeps the last **500** log events and powers the admin endpoint:

   ```
   GET  /api/admin/logs         # Admin, SuperAdmin
   GET  /api/admin/logs/summary
   ```

   Filters: level, free-text search, UTC time range, "only errors".

The `ShortSourceContextEnricher` collapses namespaces (e.g. `LMS.Application.Features.Auth.Commands.Login.LoginCommandHandler`) to a short label. `RequestLoggingMiddleware` adds `CorrelationId`, elapsed milliseconds, status code, and a friendly user label on every request log.

---

## Authorization Policies

Defined in `Program.cs` via `AddAuthorization`:

| Policy | Allowed Roles |
|--------|---------------|
| `ManageCourses` | Instructor, Admin, SuperAdmin |
| `GetInstructorCourses` | Instructor, Admin, SuperAdmin |
| `ManageQuiz` / `ManageQuestion` / `ManageAnswer` / `ManageQuizAttempts` | Instructor, Admin, SuperAdmin |
| `ManageCategory` | Admin, SuperAdmin |
| `ReadQuiz` / `ReadAnswer` / `ReadCourse` / `ManageSubmit` / `Toggle` | any authenticated user |

Controller-level `[Authorize]` and `[AllowAnonymous]` further refine access per endpoint. Admin-only surfaces (`/api/admin/...`, `LogsController`) are gated by `[Authorize(Roles = "Admin,SuperAdmin")]`.

---

## Roadmap

- [ ] Add **refresh-token rotation** tracking & reuse-detection events.
- [ ] Wire **SignalR** notification hub (service already registered in `Program.cs`).
- [ ] Introduce a **Caching** layer for catalog queries.
- [ ] Email notifications (folder already reserved in `LMS.Infrastructure/Services/Email/`).
- [ ] File storage abstraction for lesson media (`LMS.Infrastructure/Services/Storage/`).
- [ ] Containerization (Docker) and CI/CD pipeline definitions (`.github/workflows/`).
- [ ] Integration tests against a Testcontainers SQL Server.
- [ ] Replace the in-memory log buffer with a structured log query (e.g. Seq / Elasticsearch).

---

_Maintained by the LMS backend team._
