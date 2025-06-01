using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using TelegramBot.Domain.Models;
using TelegramBot.Domain.Services;

namespace TelegramBot.Infrastructure.HttpClients;

public class UserServiceClient : IUserService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<UserServiceClient> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public UserServiceClient(HttpClient httpClient, ILogger<UserServiceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public async Task<Result<UserDto>> RegisterAsync(RegisterUserDto registerDto)
    {
        try
        {
            var json = JsonSerializer.Serialize(registerDto, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/api/users/register", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var user = JsonSerializer.Deserialize<UserDto>(responseContent, _jsonOptions);
                return Result<UserDto>.Success(user!);
            }

            _logger.LogWarning("User registration failed. Status: {StatusCode}, Content: {Content}",
                response.StatusCode, responseContent);

            var errorResponse = TryDeserializeError(responseContent);
            return Result<UserDto>.Failure(errorResponse ?? "Ошибка регистрации пользователя");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during user registration");
            return Result<UserDto>.Failure("Ошибка подключения к сервису пользователей");
        }
    }

    public async Task<Result<AuthTokenDto>> LoginAsync(LoginDto loginDto)
    {
        try
        {
            var json = JsonSerializer.Serialize(loginDto, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/api/auth/login", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var token = JsonSerializer.Deserialize<AuthTokenDto>(responseContent, _jsonOptions);
                return Result<AuthTokenDto>.Success(token!);
            }

            _logger.LogWarning("User login failed. Status: {StatusCode}, Content: {Content}",
                response.StatusCode, responseContent);

            var errorResponse = TryDeserializeError(responseContent);
            return Result<AuthTokenDto>.Failure(errorResponse ?? "Ошибка входа в систему");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during user login");
            return Result<AuthTokenDto>.Failure("Ошибка подключения к сервису аутентификации");
        }
    }

    public async Task<Result<UserDto>> GetUserByIdAsync(Guid userId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/users/{userId}");
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var user = JsonSerializer.Deserialize<UserDto>(responseContent, _jsonOptions);
                return Result<UserDto>.Success(user!);
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return Result<UserDto>.Failure("Пользователь не найден");

            _logger.LogWarning("Get user failed. Status: {StatusCode}, Content: {Content}",
                response.StatusCode, responseContent);

            return Result<UserDto>.Failure("Ошибка получения данных пользователя");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user by id: {UserId}", userId);
            return Result<UserDto>.Failure("Ошибка подключения к сервису пользователей");
        }
    }

    private string? TryDeserializeError(string content)
    {
        try
        {
            using var document = JsonDocument.Parse(content);
            if (document.RootElement.TryGetProperty("message", out var messageElement))
                return messageElement.GetString();
            if (document.RootElement.TryGetProperty("error", out var errorElement))
                return errorElement.GetString();
            return null;
        }
        catch
        {
            return null;
        }
    }
}