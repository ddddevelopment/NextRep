using Microsoft.EntityFrameworkCore;
using Users.Application.Services;
using Users.DAL;
using Users.DAL.Mappings;
using Users.DAL.Repositories;
using Users.Domain.Repositories;
using Users.Domain.Services;
using Users.GrpcService.Mappings;
using Users.GrpcService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddAutoMapper(typeof(GrpcMappingProfile), typeof(DALMappingProfile));
builder.Services.AddDbContext<UsersDbContext>(options => {
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL"));
});
builder.Services.AddScoped<IUsersService, UsersService>();
builder.Services.AddScoped<IUsersRepository, UsersEFRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<UsersGrpcService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
