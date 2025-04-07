using Microsoft.EntityFrameworkCore;
using Users.Api.Mappings;
using Users.Application.Services;
using Users.DAL;
using Users.DAL.Mappings;
using Users.DAL.Repositories;
using Users.Domain.Repositories;
using Users.Domain.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(ApiMappingProfile), typeof(DALMappingProfile));
builder.Services.AddDbContext<UsersDbContext>(options => {
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL"));
});
builder.Services.AddScoped<IUsersRepository, UsersEFRepository>();
builder.Services.AddScoped<IUsersService, UsersService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(options => {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
