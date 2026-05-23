using Serilog;
using LMS.API.Extensions;
using LMS.API.Middleware;
using LMS.Application.Common.Interfaces;
using LMS.Application.DependencyInjection;
using LMS.Infrastructure.DependencyInjection;
using LMS.Infrastructure.Persistence.Seeders;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Serilog setup must happen before the rest of the host configuration.
builder.AddSerilogLogging("LMS.API");
var logger = Log.ForContext("SourceContext", "Startup");

try
{
    logger.Information("Starting LMS API application");

    // ── Controllers ───────────────────────────────────────────────
    builder.Services.AddControllers();

    // ── CORS ──────────────────────────────────────────────────────
    var allowedOrigins = builder.Configuration
        .GetSection("Cors:AllowedOrigins")
        .Get<string[]>() ?? [];

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("LmsPolicy", policy =>
            policy
                .WithOrigins(allowedOrigins)
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials());
    });

    builder.Services.AddHttpContextAccessor();
    builder.Services.AddEndpointsApiExplorer();

    // ── SignalR ───────────────────────────────────────────────────
    builder.Services.AddSignalR();

    // ── Clean Architecture layers ─────────────────────────────────
    builder.Services.AddApplication();
    builder.Services.AddInfrastructure(builder.Configuration);

    // ── Current user service ──────────────────────────────────────
    builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

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

    // ── Authorization Policies ────────────────────────────────────
    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("ManageCourses", policy => policy.RequireRole("Instructor", "Admin", "SuperAdmin"));
        options.AddPolicy("GetInstructorCourses", policy => policy.RequireRole("Instructor", "Admin", "SuperAdmin"));
        options.AddPolicy("ManageQuiz", policy => policy.RequireRole("Instructor", "Admin", "SuperAdmin"));
        options.AddPolicy("ManageQuestion", policy => policy.RequireRole("Instructor", "Admin", "SuperAdmin"));
        options.AddPolicy("ManageAnswer", policy => policy.RequireRole("Instructor", "Admin", "SuperAdmin"));
        options.AddPolicy("ManageCategory", policy => policy.RequireRole("Admin", "SuperAdmin"));
        options.AddPolicy("ReadQuiz", policy => policy.RequireAuthenticatedUser());
        options.AddPolicy("ReadAnswer", policy => policy.RequireAuthenticatedUser());
        options.AddPolicy("ReadCourse", policy => policy.RequireAuthenticatedUser());
        options.AddPolicy("ManageSubmit", policy => policy.RequireAuthenticatedUser());
        options.AddPolicy("Toggle", policy => policy.RequireAuthenticatedUser());
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

        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        if (File.Exists(xmlPath))
            options.IncludeXmlComments(xmlPath);
    });

    // ── Global exception handler ──────────────────────────────────
    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    builder.Services.AddProblemDetails();

    // ════════════════════════════════════════════════════════════
    var app = builder.Build();
    // ════════════════════════════════════════════════════════════

    logger.Information("Running in {Environment} environment", app.Environment.EnvironmentName);

    // ── Migrate and seed on startup (development only) ────────────
    if (app.Environment.IsDevelopment())
    {
        logger.Information("Applying development database migrations and seed data");
        try
        {
            await app.Services.MigrateAndSeedAsync();
            logger.Information("Database migration and seed data completed successfully");
        }
        catch (Exception ex)
        {
            logger.Fatal(ex, "Database migration and seed data failed during startup");
            throw;
        }
    }

    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "LMS API v1");
        c.RoutePrefix = string.Empty;
    });

    // ── Middleware pipeline — ORDER MATTERS ───────────────────────
    app.UseExceptionHandler();
    app.UseHttpsRedirection();
    app.UseCors("LmsPolicy");                    // ← MUST be before UseAuthentication
    app.UseAuthentication();
    app.UseMiddleware<RequestLoggingMiddleware>();
    app.UseAuthorization();
    app.MapControllers();

    logger.Information("LMS API is ready to accept requests");
    await app.RunAsync();
}
catch (Exception ex)
{
    logger.Fatal(ex, "Application terminated unexpectedly");
    throw;
}
finally
{
    Log.CloseAndFlush();
}