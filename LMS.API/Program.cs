using LMS.API.Extensions;
using LMS.API.Middleware;
using LMS.Application.Common.Interfaces;
using LMS.Application.DependencyInjection;
using LMS.Domain.Interfaces.Repositories;
using LMS.Infrastructure.DependencyInjection;
using LMS.Infrastructure.Persistence.Repositories;
using LMS.Infrastructure.Persistence.Seeders;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ── Controllers ───────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddEndpointsApiExplorer();

// ── MediatR ─────────────────────────────────────────────────
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

// ── SignalR ──────────────────────────────────────
builder.Services.AddSignalR();

// ── Clean Architecture layers ─────────────────────────────────
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// ── Current user service ──────────────────────────────────────
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

// ── Repositories ─────────────────────────────────────────────
builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddScoped<ISectionRepository, SectionRepository>();
builder.Services.AddScoped<ILessonRepository, LessonRepository>();

// ── JWT Authentication ────────────────────────────────────────
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                                           Encoding.UTF8.GetBytes(
                                               builder.Configuration["Jwt:Secret"]!)),
            ClockSkew = TimeSpan.Zero
        };
    });

// Authorization Policy
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CreateCourse", policy =>
        policy.RequireRole("Instructor", "Admin"));

    options.AddPolicy("UpdateCourse", policy =>
        policy.RequireRole("Instructor", "Admin"));

    options.AddPolicy("DeleteCourse", policy =>
        policy.RequireRole("Instructor", "Admin"));

    options.AddPolicy("ReadCourse", policy =>
        policy.RequireAuthenticatedUser());

    options.AddPolicy("PublishCourse", policy =>
    policy.RequireRole("Instructor", "Admin"));

    options.AddPolicy("ArchiveCourse", policy =>
        policy.RequireRole("Instructor", "Admin"));

    options.AddPolicy("GetInstructorCourses", policy =>
        policy.RequireRole("Instructor", "Admin"));
});

// ── Swagger with JWT support ──────────────────────────────────
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "LMS API",
        Version = "v1",
        Description = "Learning Management System API"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter: Bearer {your token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id   = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    // Include XML comments from the API project
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

// ── Global exception handler ──────────────────────────────────
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// ════════════════════════════════════════════════════════════
var app = builder.Build();
// ════════════════════════════════════════════════════════════

// ── Migrate and seed on startup (development only) ────────────
var logger = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger("Program");
logger.LogInformation("🔧 Application Environment: {Environment}", app.Environment.EnvironmentName);

if (app.Environment.IsDevelopment())
{
    logger.LogInformation("🗄️ Starting database migration and seeding...");
    try
    {
        await app.Services.MigrateAndSeedAsync();
        logger.LogInformation("✅ Database migration and seeding completed successfully!");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "❌ Error during database migration and seeding");
        throw;
    }
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "LMS API v1");
    c.RoutePrefix = string.Empty;     // ← This makes Swagger appear at the ROOT URL
});

//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI(c =>
//    {
//        c.SwaggerEndpoint("/swagger/v1/swagger.json", "LMS API v1");
//        c.RoutePrefix = string.Empty;
//    });
//}

app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();






