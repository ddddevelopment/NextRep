using System.Text;
using Auth.Application.Services;
using Auth.DAL.EF.Repositories;
using Auth.Domain.Models;
using Auth.Domain.Repositories;
using Auth.Domain.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Grpc.Net.Client;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

JwtSettings? jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
builder.Services.AddSingleton(jwtSettings);

builder.Services.AddAutoMapper(typeof(Program).Assembly, typeof(RefreshTokenRepository).Assembly);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings?.Issuer,
        ValidAudience = jwtSettings?.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings?.Secret))
    };
});

// Регистрация gRPC клиента для взаимодействия с сервисом пользователей
string usersServiceUrl = builder.Configuration["UsersService:Url"] ?? "http://localhost:5011";
builder.Services.AddSingleton(services => 
{
    return GrpcChannel.ForAddress(usersServiceUrl);
});

builder.Services.AddScoped<IAuthService, JwtAuthService>();
builder.Services.AddScoped<IUserServiceClient, UsersServiceGrpcClient>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
