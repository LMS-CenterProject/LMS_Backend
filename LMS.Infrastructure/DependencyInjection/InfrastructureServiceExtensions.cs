using LMS.Application.Common.Interfaces;
using LMS.Application.Common.Settings;
using LMS.Domain.Interfaces;
using LMS.Domain.Interfaces.Repositories;
using LMS.Infrastructure.Persistence;
using LMS.Infrastructure.Persistence.Repositories;
using LMS.Infrastructure.Services.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace LMS.Infrastructure.DependencyInjection
{
    public static class InfrastructureServiceExtensions
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration config)
        {
            // Database
            services.AddDbContext<LMSDbContext>(options =>
                options.UseSqlServer(
                    config.GetConnectionString("DefaultConnection")));

            services.AddRepositories();

            // Unit of Work
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Auth services
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<IGoogleAuthService, GoogleAuthService>();

            services.Configure<JwtSettings>(config.GetSection(JwtSettings.SectionName));

            return services;
        }

        private static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            // Application handlers inject repository interfaces directly, so wire
            // up the concrete repositories in addition to the unit of work.
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            var repositoryInterfaceNamespace = typeof(IRepository<>).Namespace;
            var repositoryTypes = typeof(UnitOfWork).Assembly
                .GetTypes()
                .Where(type =>
                    type is { IsClass: true, IsAbstract: false } &&
                    !type.IsGenericTypeDefinition &&
                    type.Name.EndsWith("Repository", StringComparison.Ordinal));

            foreach (var implementationType in repositoryTypes)
            {
                var repositoryInterfaces = implementationType
                    .GetInterfaces()
                    .Where(@interface =>
                        @interface.IsInterface &&
                        @interface.Namespace == repositoryInterfaceNamespace &&
                        !(@interface.IsGenericType &&
                          @interface.GetGenericTypeDefinition() == typeof(IRepository<>)));

                foreach (var repositoryInterface in repositoryInterfaces)
                {
                    services.AddScoped(repositoryInterface, implementationType);
                }
            }

            return services;
        }
    }
}
