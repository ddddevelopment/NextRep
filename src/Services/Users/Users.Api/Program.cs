using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Users.Api.Mappings;
using Users.Api.Models;
using Users.Application.Services;
using Users.DAL;
using Users.DAL.Mappings;
using Users.DAL.Repositories;
using Users.Domain.Repositories;
using Users.Domain.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(typeof(ApiMappingProfile), typeof(DALMappingProfile));

builder.Services.AddDbContext<UsersDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL"))
        .LogTo(message => {
            if (message.Contains("Executed DbCommand") && message.Contains("pg_class") == false) {
                Log.Logger.Debug(message);
            }
        }, LogLevel.Debug);
});

builder.Services.AddScoped<IUsersRepository, UsersEFRepository>();
builder.Services.AddScoped<IUsersService, UsersService>();

builder.Services.AddValidatorsFromAssemblyContaining<UserCreateRequest>();
builder.Services.AddFluentValidationAutoValidation();

Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).CreateLogger();
builder.Services.AddSerilog(Log.Logger);

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService(serviceName: "usersapi"))
    .WithMetrics(metrics => {
        metrics.AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddPrometheusExporter();
    })
    .WithTracing(tracing => {
        tracing.AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddOtlpExporter(options => {
                options.Endpoint = new Uri(builder.Configuration["Jaeger:Uri"]);
            });
    });

try
{
    Log.Information("Starting Users.Api application");
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

    app.UseOpenTelemetryPrometheusScrapingEndpoint();

    app.Run();
}
catch (Exception exception) {
    Log.Fatal(exception, "Application terminated unexpectedly");
}
finally {
    Log.CloseAndFlush();
}