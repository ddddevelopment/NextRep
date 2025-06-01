using Microsoft.Extensions.DependencyInjection;
using MediatR;
using System.Reflection;
using TelegramBot.Application.Services;

namespace TelegramBot.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Регистрируем MediatR и все обработчики из Application слоя
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        services.AddScoped<ITelegramBotService, TelegramBotService>();
        return services;
    }
}
