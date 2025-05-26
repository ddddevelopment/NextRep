using Microsoft.EntityFrameworkCore;
using Workouts.DAL;
using Workouts.DAL.EF.Repositories;
using Workouts.Domain.Repositories;
using Workouts.Application.Services;
using Workouts.Domain.Services;
using Workouts.Api.Mappings;
using Workouts.DAL.EF.Mappings;
using FluentValidation;
using FluentValidation.AspNetCore;
using Workouts.Api.Validators;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<WorkoutsDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL")));

builder.Services.AddScoped<IWorkoutsRepository, WorkoutsEFRepository>();
builder.Services.AddScoped<IWorkoutsService, WorkoutsService>();
builder.Services.AddScoped<IExerciseInfosRepository, ExerciseInfosEFRepository>();
builder.Services.AddScoped<IExerciseInfosService, ExerciseInfosService>();
builder.Services.AddAutoMapper(
    typeof(ApiMappingProfile),
    typeof(DALMappingProfile)
);

builder.Services.AddValidatorsFromAssemblyContaining<WorkoutCreateDtoValidator>();
builder.Services.AddFluentValidationAutoValidation();

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
