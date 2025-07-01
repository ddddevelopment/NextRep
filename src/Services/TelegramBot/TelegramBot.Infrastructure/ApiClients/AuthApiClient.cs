using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using TelegramBot.Domain.Models;
using TelegramBot.Domain.Services;

namespace TelegramBot.Infrastructure.ApiClients;

public class AuthApiClient : IAuthApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AuthApiClient>? _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public AuthApiClient(HttpClient httpClient, ILogger<AuthApiClient>? logger = null)
    {
        _httpClient = httpClient;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<Result<AuthResponseDto>> LoginAsync(LoginDto login, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger?.LogInformation("Attempting login for user: {Email}", login.Email);
            
            var json = JsonSerializer.Serialize(login, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync("auth/login", content, cancellationToken);
            
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var authResponse = JsonSerializer.Deserialize<AuthResponseDto>(responseContent, _jsonOptions);
            
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                var errorMessage = authResponse?.ErrorMessage ?? "Неверный email или пароль";
                return Result<AuthResponseDto>.Invalid(errorMessage);
            }

            response.EnsureSuccessStatusCode();
            
            return authResponse != null 
                ? Result<AuthResponseDto>.Success(authResponse)
                : Result<AuthResponseDto>.Failure("Ошибка десериализации ответа авторизации");
        }
        catch (HttpRequestException ex)
        {
            _logger?.LogError(ex, "HTTP error during login for user: {Email}", login.Email);
            return Result<AuthResponseDto>.Failure("Ошибка сети при авторизации");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Unexpected error during login for user: {Email}", login.Email);
            return Result<AuthResponseDto>.Failure("Неожиданная ошибка при авторизации");
        }
    }

    public async Task<Result<AuthResponseDto>> RegisterAsync(RegisterDto register, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger?.LogInformation("Attempting registration for user: {Email}", register.Email);
            
            var json = JsonSerializer.Serialize(register, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync("auth/register", content, cancellationToken);
            
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var authResponse = JsonSerializer.Deserialize<AuthResponseDto>(responseContent, _jsonOptions);
            
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                var errorMessage = authResponse?.ErrorMessage ?? "Ошибка регистрации";
                return Result<AuthResponseDto>.Conflict(errorMessage);
            }

            response.EnsureSuccessStatusCode();
            
            return authResponse != null 
                ? Result<AuthResponseDto>.Success(authResponse)
                : Result<AuthResponseDto>.Failure("Ошибка десериализации ответа регистрации");
        }
        catch (HttpRequestException ex)
        {
            _logger?.LogError(ex, "HTTP error during registration for user: {Email}", register.Email);
            return Result<AuthResponseDto>.Failure("Ошибка сети при регистрации");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Unexpected error during registration for user: {Email}", register.Email);
            return Result<AuthResponseDto>.Failure("Неожиданная ошибка при регистрации");
        }
    }
} 