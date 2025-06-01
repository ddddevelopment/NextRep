using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using TelegramBot.Domain.Models;
using TelegramBot.Domain.Services;

namespace TelegramBot.Infrastructure.HttpClients;

public class WorkoutServiceClient : IWorkoutService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<WorkoutServiceClient> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public WorkoutServiceClient(HttpClient httpClient, ILogger<WorkoutServiceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public async Task<Result<WorkoutDto>> CreateWorkoutAsync(Guid userId, CreateWorkoutDto workout)
    {
        try
        {
            var json = JsonSerializer.Serialize(workout, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Add("UserId", userId.ToString());

            var response = await _httpClient.PostAsync("/api/workouts", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var createdWorkout = JsonSerializer.Deserialize<WorkoutDto>(responseContent, _jsonOptions);
                return Result<WorkoutDto>.Success(createdWorkout!);
            }

            _logger.LogWarning("Create workout failed. Status: {StatusCode}, Content: {Content}",
                response.StatusCode, responseContent);

            var errorResponse = TryDeserializeError(responseContent);
            return Result<WorkoutDto>.Failure(errorResponse ?? "Ошибка создания тренировки");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating workout for user: {UserId}", userId);
            return Result<WorkoutDto>.Failure("Ошибка подключения к сервису тренировок");
        }
        finally
        {
            _httpClient.DefaultRequestHeaders.Remove("UserId");
        }
    }

    public async Task<Result<List<WorkoutDto>>> GetUserWorkoutsAsync(Guid userId, int page, int pageSize)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Add("UserId", userId.ToString());

            var response = await _httpClient.GetAsync($"/api/workouts?page={page}&pageSize={pageSize}");
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var workouts = JsonSerializer.Deserialize<List<WorkoutDto>>(responseContent, _jsonOptions);
                return Result<List<WorkoutDto>>.Success(workouts ?? new List<WorkoutDto>());
            }

            _logger.LogWarning("Get workouts failed. Status: {StatusCode}, Content: {Content}",
                response.StatusCode, responseContent);

            return Result<List<WorkoutDto>>.Failure("Ошибка получения тренировок");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting workouts for user: {UserId}", userId);
            return Result<List<WorkoutDto>>.Failure("Ошибка подключения к сервису тренировок");
        }
        finally
        {
            _httpClient.DefaultRequestHeaders.Remove("UserId");
        }
    }

    public async Task<Result<WorkoutStatsDto>> GetWorkoutStatsAsync(Guid userId)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Add("UserId", userId.ToString());

            var response = await _httpClient.GetAsync("/api/workouts/stats");
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var stats = JsonSerializer.Deserialize<WorkoutStatsDto>(responseContent, _jsonOptions);
                return Result<WorkoutStatsDto>.Success(stats!);
            }

            _logger.LogWarning("Get workout stats failed. Status: {StatusCode}, Content: {Content}",
                response.StatusCode, responseContent);

            return Result<WorkoutStatsDto>.Failure("Ошибка получения статистики");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting workout stats for user: {UserId}", userId);
            return Result<WorkoutStatsDto>.Failure("Ошибка подключения к сервису тренировок");
        }
        finally
        {
            _httpClient.DefaultRequestHeaders.Remove("UserId");
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