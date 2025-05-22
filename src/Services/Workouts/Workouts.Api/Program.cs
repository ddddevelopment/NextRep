using Microsoft.EntityFrameworkCore;
using Workouts.DAL;
using Workouts.DAL.Repositories;
using Workouts.Domain.Repositories;
using Workouts.Application.Services;
using Workouts.Domain.Services;
using Workouts.Api.Mappings;
using Workouts.DAL.Mappings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<WorkoutsDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL")));

builder.Services.AddScoped<IWorkoutsRepository, WorkoutsEFRepository>();
builder.Services.AddScoped<IWorkoutsService, WorkoutsService>();
builder.Services.AddAutoMapper(typeof(ApiMappingProfile), typeof(DALMappingProfile));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
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
app.Run();
