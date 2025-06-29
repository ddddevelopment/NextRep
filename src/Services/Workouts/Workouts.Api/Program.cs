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
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

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
builder.Services.AddScoped<IExercisesRepository, ExercisesEFRepository>();
builder.Services.AddScoped<IExercisesService, ExercisesService>();
builder.Services.AddScoped<ISetsRepository, SetsEFRepository>();
builder.Services.AddScoped<ISetsService, SetsService>();
builder.Services.AddAutoMapper(
    typeof(ApiMappingProfile),
    typeof(DALMappingProfile)
);

builder.Services.AddValidatorsFromAssemblyContaining<WorkoutCreateRequestValidator>();
builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

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
