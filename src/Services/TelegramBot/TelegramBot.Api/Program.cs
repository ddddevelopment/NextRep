using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Telegram.Bot;
using TelegramBot.Application.Commands.ProcessUpdate;
using TelegramBot.Application.Services;
using TelegramBot.Domain.Services;
using TelegramBot.Infrastructure.ApiClients;
using TelegramBot.Infrastructure.Mappings;
using TelegramBot.Infrastructure.Services;
using TelegramBot.Infrastructure.Settings;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

// AutoMapper
builder.Services.AddAutoMapper(typeof(TelegramMappingProfile));

// MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ProcessUpdateCommand).Assembly));

// Application Services
builder.Services.AddScoped<ICommandRouter, CommandRouter>();
builder.Services.AddSingleton<IUserSessionService, InMemoryUserSessionService>();

// HTTP Clients for API integration
builder.Services.AddHttpClient<IUsersApiClient, UsersApiClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiClients:UsersApi"] ?? "http://localhost:5001");
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddHttpClient<IAuthApiClient, AuthApiClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiClients:AuthApi"] ?? "http://localhost:7080");
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddHttpClient<IWorkoutsApiClient, WorkoutsApiClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiClients:WorkoutsApi"] ?? "http://localhost:5003");
    client.Timeout = TimeSpan.FromSeconds(30);
});

// Telegram Bot
builder.Services.Configure<TelegramBotSettings>(builder.Configuration.GetSection(TelegramBotSettings.SectionName));
builder.Services.AddSingleton<ITelegramBotClient>(provider =>
{
    var settings = builder.Configuration.GetSection(TelegramBotSettings.SectionName).Get<TelegramBotSettings>();
    return new TelegramBotClient(settings?.Token ?? throw new InvalidOperationException("TelegramBot:Token not configured"));
});
builder.Services.AddScoped<ITelegramBotService, TelegramBotService>();

// Polling background service (only starts if WebhookUrl is empty)
builder.Services.AddHostedService<TelegramPollingBackgroundService>();

// FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<ProcessUpdateCommand>();
builder.Services.AddFluentValidationAutoValidation();

// Serilog
Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).CreateLogger();
builder.Services.AddSerilog(Log.Logger);

// OpenTelemetry
builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService(serviceName: "telegrambotapi"))
    .WithMetrics(metrics =>
    {
        metrics.AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation();
    })
    .WithTracing(tracing =>
    {
        tracing.AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation();
    });

try
{
    Log.Information("Starting TelegramBot.Api application");
    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        Log.Information("Application is running in Development environment");
        app.MapOpenApi();
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
            options.RoutePrefix = string.Empty;
        });
    }

    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();

    using var scope = app.Services.CreateScope();
    var botService = scope.ServiceProvider.GetRequiredService<ITelegramBotService>();
    var settings = scope.ServiceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptions<TelegramBotSettings>>().Value;

    if (!string.IsNullOrEmpty(settings.WebhookUrl))
    {
        await botService.SetWebhookAsync($"{settings.WebhookUrl}/api/webhook");
        Log.Information("Webhook set to {WebhookUrl}/api/webhook", settings.WebhookUrl);
    }

    app.Run();
}
catch (Exception exception)
{
    Log.Fatal(exception, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
