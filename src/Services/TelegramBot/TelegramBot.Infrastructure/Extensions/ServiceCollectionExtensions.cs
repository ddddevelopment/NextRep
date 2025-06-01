using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using Microsoft.Extensions.Http;
using System;
using TelegramBot.Domain.Services;
using TelegramBot.Infrastructure.Data;
using TelegramBot.Infrastructure.Services;
using TelegramBot.Infrastructure.HttpClients;

namespace TelegramBot.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Database
            services.AddDbContext<TelegramBotDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly("GymTracker.TelegramBot.Infrastructure")));

            // Services
            services.AddScoped<ITelegramUserService, TelegramUserService>();

            // HTTP Clients
            services.AddHttpClient<IUserService, UserServiceClient>(client =>
            {
                client.BaseAddress = new Uri(configuration["Services:UserService:BaseUrl"]!);
                client.Timeout = TimeSpan.FromSeconds(30);
            });

            services.AddHttpClient<IWorkoutService, WorkoutServiceClient>(client =>
            {
                client.BaseAddress = new Uri(configuration["Services:WorkoutService:BaseUrl"]!);
                client.Timeout = TimeSpan.FromSeconds(30);
            });

            return services;
        }
    }
}