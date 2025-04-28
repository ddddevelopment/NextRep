using System.Text;
using Auth.Application.Services;
using Auth.Domain.Models;
using Auth.Domain.Services;
using Auth.Infrastructure.Users.Mappings;
using Auth.Infrastructure.Users.Services;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(typeof(GrpcMappingProfile));

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
JwtSettings jwtSettings = builder.Services.BuildServiceProvider().GetRequiredService<IOptions<JwtSettings>>().Value;
builder.Services.AddSingleton(jwtSettings);

var grpcChannel = GrpcChannel.ForAddress(builder.Configuration["UsersGrpcService"]);
builder.Services.AddSingleton(grpcChannel);

builder.Services.AddScoped<IAuthService, JwtAuthService>();
builder.Services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
builder.Services.AddScoped<IUserServiceClient, UsersGrpcServiceClient>();

builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options => {
    options.TokenValidationParameters = new TokenValidationParameters {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings?.Issuer,
        ValidAudience = jwtSettings?.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings?.SecretKey))
    };
});

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
