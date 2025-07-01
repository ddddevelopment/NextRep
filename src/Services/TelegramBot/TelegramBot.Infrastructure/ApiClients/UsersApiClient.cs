using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using TelegramBot.Domain.Models;
using TelegramBot.Domain.Services;

namespace TelegramBot.Infrastructure.ApiClients;

public class UsersApiClient : IUsersApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<UsersApiClient>? _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public UsersApiClient(HttpClient httpClient, ILogger<UsersApiClient>? logger = null)
    {
        _httpClient = httpClient;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<Result<UserDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger?.LogInformation("Getting user by ID: {UserId}", id);
            
            var response = await _httpClient.GetAsync($"users/{id}", cancellationToken);
            
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return Result<UserDto>.NotFound("Пользователь не найден");
            }

            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var user = JsonSerializer.Deserialize<UserDto>(content, _jsonOptions);
            
            return user != null 
                ? Result<UserDto>.Success(user)
                : Result<UserDto>.Failure("Ошибка десериализации данных пользователя");
        }
        catch (HttpRequestException ex)
        {
            _logger?.LogError(ex, "HTTP error while getting user by ID: {UserId}", id);
            return Result<UserDto>.Failure("Ошибка сети при получении пользователя");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Unexpected error while getting user by ID: {UserId}", id);
            return Result<UserDto>.Failure("Неожиданная ошибка при получении пользователя");
        }
    }

    public async Task<Result<UserDto>> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger?.LogInformation("Getting user by email: {Email}", email);
            
            var response = await _httpClient.GetAsync($"users/by-email/{Uri.EscapeDataString(email)}", cancellationToken);
            
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return Result<UserDto>.NotFound("Пользователь не найден");
            }

            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var user = JsonSerializer.Deserialize<UserDto>(content, _jsonOptions);
            
            return user != null 
                ? Result<UserDto>.Success(user)
                : Result<UserDto>.Failure("Ошибка десериализации данных пользователя");
        }
        catch (HttpRequestException ex)
        {
            _logger?.LogError(ex, "HTTP error while getting user by email: {Email}", email);
            return Result<UserDto>.Failure("Ошибка сети при получении пользователя");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Unexpected error while getting user by email: {Email}", email);
            return Result<UserDto>.Failure("Неожиданная ошибка при получении пользователя");
        }
    }

    public async Task<Result<IEnumerable<UserDto>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger?.LogInformation("Getting all users");
            
            var response = await _httpClient.GetAsync("users", cancellationToken);
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var users = JsonSerializer.Deserialize<IEnumerable<UserDto>>(content, _jsonOptions);
            
            return users != null 
                ? Result<IEnumerable<UserDto>>.Success(users)
                : Result<IEnumerable<UserDto>>.Failure("Ошибка десериализации списка пользователей");
        }
        catch (HttpRequestException ex)
        {
            _logger?.LogError(ex, "HTTP error while getting all users");
            return Result<IEnumerable<UserDto>>.Failure("Ошибка сети при получении списка пользователей");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Unexpected error while getting all users");
            return Result<IEnumerable<UserDto>>.Failure("Неожиданная ошибка при получении списка пользователей");
        }
    }

    public async Task<Result<Guid>> CreateAsync(UserCreateDto user, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger?.LogInformation("Creating user: {Email}", user.Email);
            
            var json = JsonSerializer.Serialize(user, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync("users", content, cancellationToken);
            
            if (response.StatusCode == HttpStatusCode.Conflict)
            {
                return Result<Guid>.Conflict("Пользователь с таким email уже существует");
            }

            response.EnsureSuccessStatusCode();
            
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            
            if (Guid.TryParse(responseContent.Trim('"'), out var userId))
            {
                return Result<Guid>.Success(userId);
            }
            
            return Result<Guid>.Failure("Ошибка получения ID созданного пользователя");
        }
        catch (HttpRequestException ex)
        {
            _logger?.LogError(ex, "HTTP error while creating user: {Email}", user.Email);
            return Result<Guid>.Failure("Ошибка сети при создании пользователя");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Unexpected error while creating user: {Email}", user.Email);
            return Result<Guid>.Failure("Неожиданная ошибка при создании пользователя");
        }
    }

    public async Task<Result> UpdateAsync(UserUpdateDto user, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger?.LogInformation("Updating user: {UserId}", user.Id);
            
            var json = JsonSerializer.Serialize(user, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PutAsync("users", content, cancellationToken);
            
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return Result.NotFound("Пользователь не найден");
            }

            response.EnsureSuccessStatusCode();
            return Result.Success();
        }
        catch (HttpRequestException ex)
        {
            _logger?.LogError(ex, "HTTP error while updating user: {UserId}", user.Id);
            return Result.Failure("Ошибка сети при обновлении пользователя");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Unexpected error while updating user: {UserId}", user.Id);
            return Result.Failure("Неожиданная ошибка при обновлении пользователя");
        }
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger?.LogInformation("Deleting user: {UserId}", id);
            
            var response = await _httpClient.DeleteAsync($"users?id={id}", cancellationToken);
            
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return Result.NotFound("Пользователь не найден");
            }

            response.EnsureSuccessStatusCode();
            return Result.Success();
        }
        catch (HttpRequestException ex)
        {
            _logger?.LogError(ex, "HTTP error while deleting user: {UserId}", id);
            return Result.Failure("Ошибка сети при удалении пользователя");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Unexpected error while deleting user: {UserId}", id);
            return Result.Failure("Неожиданная ошибка при удалении пользователя");
        }
    }
} 