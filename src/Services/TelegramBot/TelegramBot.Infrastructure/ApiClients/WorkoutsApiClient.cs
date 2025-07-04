using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using TelegramBot.Domain.Models;
using TelegramBot.Domain.Services;
using System.Net.Http.Json;

namespace TelegramBot.Infrastructure.ApiClients;

public class WorkoutsApiClient : IWorkoutsApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<WorkoutsApiClient>? _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public WorkoutsApiClient(HttpClient httpClient, ILogger<WorkoutsApiClient>? logger = null)
    {
        _httpClient = httpClient;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };
    }

    private void SetAuthorizationHeader(string token)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    #region Workouts

    public async Task<Result<IEnumerable<WorkoutDto>>> GetAllWorkoutsAsync(string token, CancellationToken cancellationToken = default)
    {
        try
        {
            SetAuthorizationHeader(token);
            _logger?.LogInformation("Getting all workouts");
            
            var response = await _httpClient.GetAsync("api/workouts", cancellationToken);
            
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return Result<IEnumerable<WorkoutDto>>.Failure("Требуется авторизация");
            }

            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var workouts = JsonSerializer.Deserialize<IEnumerable<WorkoutDto>>(content, _jsonOptions);
            
            return workouts != null 
                ? Result<IEnumerable<WorkoutDto>>.Success(workouts)
                : Result<IEnumerable<WorkoutDto>>.Failure("Ошибка десериализации списка тренировок");
        }
        catch (HttpRequestException ex)
        {
            _logger?.LogError(ex, "HTTP error while getting all workouts");
            return Result<IEnumerable<WorkoutDto>>.Failure("Ошибка сети при получении списка тренировок");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Unexpected error while getting all workouts");
            return Result<IEnumerable<WorkoutDto>>.Failure("Неожиданная ошибка при получении списка тренировок");
        }
    }

    public async Task<Result<WorkoutDto>> GetWorkoutByIdAsync(Guid id, string token, CancellationToken cancellationToken = default)
    {
        try
        {
            SetAuthorizationHeader(token);
            _logger?.LogInformation("Getting workout by ID: {WorkoutId}", id);
            
            var response = await _httpClient.GetAsync($"api/workouts/{id}", cancellationToken);
            
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return Result<WorkoutDto>.NotFound("Тренировка не найдена");
            }
            
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return Result<WorkoutDto>.Failure("Требуется авторизация");
            }

            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var workout = JsonSerializer.Deserialize<WorkoutDto>(content, _jsonOptions);
            
            return workout != null 
                ? Result<WorkoutDto>.Success(workout)
                : Result<WorkoutDto>.Failure("Ошибка десериализации данных тренировки");
        }
        catch (HttpRequestException ex)
        {
            _logger?.LogError(ex, "HTTP error while getting workout by ID: {WorkoutId}", id);
            return Result<WorkoutDto>.Failure("Ошибка сети при получении тренировки");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Unexpected error while getting workout by ID: {WorkoutId}", id);
            return Result<WorkoutDto>.Failure("Неожиданная ошибка при получении тренировки");
        }
    }

    public async Task<Result> CreateWorkoutAsync(WorkoutCreateDto workout, string token, CancellationToken cancellationToken = default)
    {
        try
        {
            SetAuthorizationHeader(token);
            _logger?.LogInformation("Creating workout started at {StartTime}", workout.StartTime);
            
            var json = JsonSerializer.Serialize(workout, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync("api/workouts", content, cancellationToken);
            
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return Result.Failure("Требуется авторизация");
            }
            
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger?.LogWarning("Bad request while creating workout. API returned: {ErrorContent}", errorContent);
                return Result.Failure($"Ошибка валидации: {errorContent}");
            }

            response.EnsureSuccessStatusCode();
            
            return Result.Success();
        }
        catch (HttpRequestException ex)
        {
            _logger?.LogError(ex, "HTTP error while creating workout");
            return Result.Failure("Ошибка сети при создании тренировки");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Unexpected error while creating workout");
            return Result.Failure("Неожиданная ошибка при создании тренировки");
        }
    }

    public async Task<Result> UpdateWorkoutAsync(Guid id, WorkoutUpdateDto workout, string token, CancellationToken cancellationToken = default)
    {
        try
        {
            SetAuthorizationHeader(token);
            _logger?.LogInformation("Updating workout: {WorkoutId}", id);
            
            var json = JsonSerializer.Serialize(workout, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PutAsync($"api/workouts/{id}", content, cancellationToken);
            
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return Result.NotFound("Тренировка не найдена");
            }
            
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return Result.Failure("Требуется авторизация");
            }

            response.EnsureSuccessStatusCode();
            return Result.Success();
        }
        catch (HttpRequestException ex)
        {
            _logger?.LogError(ex, "HTTP error while updating workout: {WorkoutId}", id);
            return Result.Failure("Ошибка сети при обновлении тренировки");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Unexpected error while updating workout: {WorkoutId}", id);
            return Result.Failure("Неожиданная ошибка при обновлении тренировки");
        }
    }

    public async Task<Result> DeleteWorkoutAsync(Guid id, string token, CancellationToken cancellationToken = default)
    {
        try
        {
            SetAuthorizationHeader(token);
            _logger?.LogInformation("Deleting workout: {WorkoutId}", id);
            
            var response = await _httpClient.DeleteAsync($"api/workouts/{id}", cancellationToken);
            
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return Result.NotFound("Тренировка не найдена");
            }
            
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return Result.Failure("Требуется авторизация");
            }

            response.EnsureSuccessStatusCode();
            return Result.Success();
        }
        catch (HttpRequestException ex)
        {
            _logger?.LogError(ex, "HTTP error while deleting workout: {WorkoutId}", id);
            return Result.Failure("Ошибка сети при удалении тренировки");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Unexpected error while deleting workout: {WorkoutId}", id);
            return Result.Failure("Неожиданная ошибка при удалении тренировки");
        }
    }

    #endregion

    #region Exercises

    public async Task<Result<IEnumerable<ExerciseDto>>> GetExercisesByWorkoutIdAsync(Guid workoutId, string token, CancellationToken cancellationToken = default)
    {
        try
        {
            SetAuthorizationHeader(token);
            _logger?.LogInformation("Getting exercises for workout: {WorkoutId}", workoutId);
            
            var response = await _httpClient.GetAsync($"api/workouts/{workoutId}/exercises", cancellationToken);
            
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return Result<IEnumerable<ExerciseDto>>.Failure("Требуется авторизация");
            }

            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var exercises = JsonSerializer.Deserialize<IEnumerable<ExerciseDto>>(content, _jsonOptions);
            
            return exercises != null 
                ? Result<IEnumerable<ExerciseDto>>.Success(exercises)
                : Result<IEnumerable<ExerciseDto>>.Failure("Ошибка десериализации списка упражнений");
        }
        catch (HttpRequestException ex)
        {
            _logger?.LogError(ex, "HTTP error while getting exercises for workout: {WorkoutId}", workoutId);
            return Result<IEnumerable<ExerciseDto>>.Failure("Ошибка сети при получении упражнений");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Unexpected error while getting exercises for workout: {WorkoutId}", workoutId);
            return Result<IEnumerable<ExerciseDto>>.Failure("Неожиданная ошибка при получении упражнений");
        }
    }

    public async Task<Result<ExerciseDto>> GetExerciseByIdAsync(Guid workoutId, Guid exerciseId, string token, CancellationToken cancellationToken = default)
    {
        try
        {
            SetAuthorizationHeader(token);
            _logger?.LogInformation("Getting exercise {ExerciseId} for workout {WorkoutId}", exerciseId, workoutId);
            
            var response = await _httpClient.GetAsync($"api/workouts/{workoutId}/exercises/{exerciseId}", cancellationToken);
            
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return Result<ExerciseDto>.NotFound("Упражнение не найдено");
            }
            
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return Result<ExerciseDto>.Failure("Требуется авторизация");
            }

            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var exercise = JsonSerializer.Deserialize<ExerciseDto>(content, _jsonOptions);
            
            return exercise != null 
                ? Result<ExerciseDto>.Success(exercise)
                : Result<ExerciseDto>.Failure("Ошибка десериализации данных упражнения");
        }
        catch (HttpRequestException ex)
        {
            _logger?.LogError(ex, "HTTP error while getting exercise {ExerciseId} for workout {WorkoutId}", exerciseId, workoutId);
            return Result<ExerciseDto>.Failure("Ошибка сети при получении упражнения");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Unexpected error while getting exercise {ExerciseId} for workout {WorkoutId}", exerciseId, workoutId);
            return Result<ExerciseDto>.Failure("Неожиданная ошибка при получении упражнения");
        }
    }

    public async Task<Result> CreateExerciseAsync(Guid workoutId, ExerciseCreateDto exercise, string token, CancellationToken cancellationToken = default)
    {
        try
        {
            SetAuthorizationHeader(token);
            _logger?.LogInformation("Creating exercise '{ExerciseName}' for workout {WorkoutId}", exercise.Name, workoutId);
            
            var response = await _httpClient.PostAsJsonAsync($"api/workouts/{workoutId}/exercises", exercise, _jsonOptions, cancellationToken);
            
            if (response.StatusCode == HttpStatusCode.Unauthorized) return Result.Failure("Требуется авторизация");

            response.EnsureSuccessStatusCode();
            return Result.Success();
        }
        catch (HttpRequestException ex)
        {
            _logger?.LogError(ex, "HTTP error while creating exercise for workout {WorkoutId}", workoutId);
            return Result.Failure("Ошибка сети при создании упражнения");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Unexpected error while creating exercise for workout {WorkoutId}", workoutId);
            return Result.Failure("Неожиданная ошибка при создании упражнения");
        }
    }

    public async Task<Result> UpdateExerciseAsync(Guid workoutId, Guid exerciseId, ExerciseUpdateDto exercise, string token, CancellationToken cancellationToken = default)
    {
        try
        {
            SetAuthorizationHeader(token);
            _logger?.LogInformation("Updating exercise {ExerciseId} for workout {WorkoutId}", exerciseId, workoutId);
            
            var json = JsonSerializer.Serialize(exercise, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PutAsync($"api/workouts/{workoutId}/exercises/{exerciseId}", content, cancellationToken);
            
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return Result.NotFound("Упражнение не найдено");
            }
            
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return Result.Failure("Требуется авторизация");
            }

            response.EnsureSuccessStatusCode();
            return Result.Success();
        }
        catch (HttpRequestException ex)
        {
            _logger?.LogError(ex, "HTTP error while updating exercise {ExerciseId} for workout {WorkoutId}", exerciseId, workoutId);
            return Result.Failure("Ошибка сети при обновлении упражнения");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Unexpected error while updating exercise {ExerciseId} for workout {WorkoutId}", exerciseId, workoutId);
            return Result.Failure("Неожиданная ошибка при обновлении упражнения");
        }
    }

    public async Task<Result> DeleteExerciseAsync(Guid workoutId, Guid exerciseId, string token, CancellationToken cancellationToken = default)
    {
        try
        {
            SetAuthorizationHeader(token);
            _logger?.LogInformation("Deleting exercise {ExerciseId} from workout {WorkoutId}", exerciseId, workoutId);
            
            var response = await _httpClient.DeleteAsync($"api/workouts/{workoutId}/exercises/{exerciseId}", cancellationToken);
            
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return Result.NotFound("Упражнение не найдено");
            }
            
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return Result.Failure("Требуется авторизация");
            }

            response.EnsureSuccessStatusCode();
            return Result.Success();
        }
        catch (HttpRequestException ex)
        {
            _logger?.LogError(ex, "HTTP error while deleting exercise {ExerciseId} from workout {WorkoutId}", exerciseId, workoutId);
            return Result.Failure("Ошибка сети при удалении упражнения");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Unexpected error while deleting exercise {ExerciseId} from workout {WorkoutId}", exerciseId, workoutId);
            return Result.Failure("Неожиданная ошибка при удалении упражнения");
        }
    }

    #endregion

    #region Sets

    public async Task<Result<IEnumerable<SetDto>>> GetSetsByExerciseIdAsync(Guid workoutId, Guid exerciseId, string token, CancellationToken cancellationToken = default)
    {
        try
        {
            SetAuthorizationHeader(token);
            _logger?.LogInformation("Getting sets for exercise {ExerciseId} in workout {WorkoutId}", exerciseId, workoutId);
            
            var response = await _httpClient.GetAsync($"api/workouts/{workoutId}/exercises/{exerciseId}/sets", cancellationToken);
            
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return Result<IEnumerable<SetDto>>.Failure("Требуется авторизация");
            }

            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var sets = JsonSerializer.Deserialize<IEnumerable<SetDto>>(content, _jsonOptions);
            
            return sets != null 
                ? Result<IEnumerable<SetDto>>.Success(sets)
                : Result<IEnumerable<SetDto>>.Failure("Ошибка десериализации списка подходов");
        }
        catch (HttpRequestException ex)
        {
            _logger?.LogError(ex, "HTTP error while getting sets for exercise {ExerciseId} in workout {WorkoutId}", exerciseId, workoutId);
            return Result<IEnumerable<SetDto>>.Failure("Ошибка сети при получении подходов");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Unexpected error while getting sets for exercise {ExerciseId} in workout {WorkoutId}", exerciseId, workoutId);
            return Result<IEnumerable<SetDto>>.Failure("Неожиданная ошибка при получении подходов");
        }
    }

    public async Task<Result<SetDto>> GetSetByIdAsync(Guid workoutId, Guid exerciseId, Guid setId, string token, CancellationToken cancellationToken = default)
    {
        try
        {
            SetAuthorizationHeader(token);
            _logger?.LogInformation("Getting set {SetId} for exercise {ExerciseId} in workout {WorkoutId}", setId, exerciseId, workoutId);
            
            var response = await _httpClient.GetAsync($"api/workouts/{workoutId}/exercises/{exerciseId}/sets/{setId}", cancellationToken);
            
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return Result<SetDto>.NotFound("Подход не найден");
            }
            
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return Result<SetDto>.Failure("Требуется авторизация");
            }

            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var set = JsonSerializer.Deserialize<SetDto>(content, _jsonOptions);
            
            return set != null 
                ? Result<SetDto>.Success(set)
                : Result<SetDto>.Failure("Ошибка десериализации данных подхода");
        }
        catch (HttpRequestException ex)
        {
            _logger?.LogError(ex, "HTTP error while getting set {SetId} for exercise {ExerciseId} in workout {WorkoutId}", setId, exerciseId, workoutId);
            return Result<SetDto>.Failure("Ошибка сети при получении подхода");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Unexpected error while getting set {SetId} for exercise {ExerciseId} in workout {WorkoutId}", setId, exerciseId, workoutId);
            return Result<SetDto>.Failure("Неожиданная ошибка при получении подхода");
        }
    }

    public async Task<Result> CreateSetAsync(Guid workoutId, Guid exerciseId, SetCreateDto set, string token, CancellationToken cancellationToken = default)
    {
        try
        {
            SetAuthorizationHeader(token);
            _logger?.LogInformation("Creating set for exercise {ExerciseId}", exerciseId);
            
            var response = await _httpClient.PostAsJsonAsync($"api/workouts/{workoutId}/exercises/{exerciseId}/sets", set, _jsonOptions, cancellationToken);
            
            if (response.StatusCode == HttpStatusCode.Unauthorized) return Result.Failure("Требуется авторизация");

            response.EnsureSuccessStatusCode();
            return Result.Success();
        }
        catch (HttpRequestException ex)
        {
            _logger?.LogError(ex, "HTTP error while creating set for exercise {ExerciseId}", exerciseId);
            return Result.Failure("Ошибка сети при создании подхода");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Unexpected error while creating set for exercise {ExerciseId}", exerciseId);
            return Result.Failure("Неожиданная ошибка при создании подхода");
        }
    }

    public async Task<Result> UpdateSetAsync(Guid workoutId, Guid exerciseId, Guid setId, SetUpdateDto set, string token, CancellationToken cancellationToken = default)
    {
        try
        {
            SetAuthorizationHeader(token);
            _logger?.LogInformation("Updating set {SetId} for exercise {ExerciseId} in workout {WorkoutId}", setId, exerciseId, workoutId);
            
            var json = JsonSerializer.Serialize(set, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PutAsync($"api/workouts/{workoutId}/exercises/{exerciseId}/sets/{setId}", content, cancellationToken);
            
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return Result.NotFound("Подход не найден");
            }
            
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return Result.Failure("Требуется авторизация");
            }

            response.EnsureSuccessStatusCode();
            return Result.Success();
        }
        catch (HttpRequestException ex)
        {
            _logger?.LogError(ex, "HTTP error while updating set {SetId} for exercise {ExerciseId} in workout {WorkoutId}", setId, exerciseId, workoutId);
            return Result.Failure("Ошибка сети при обновлении подхода");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Unexpected error while updating set {SetId} for exercise {ExerciseId} in workout {WorkoutId}", setId, exerciseId, workoutId);
            return Result.Failure("Неожиданная ошибка при обновлении подхода");
        }
    }

    public async Task<Result> DeleteSetAsync(Guid workoutId, Guid exerciseId, Guid setId, string token, CancellationToken cancellationToken = default)
    {
        try
        {
            SetAuthorizationHeader(token);
            _logger?.LogInformation("Deleting set {SetId} from exercise {ExerciseId} in workout {WorkoutId}", setId, exerciseId, workoutId);
            
            var response = await _httpClient.DeleteAsync($"api/workouts/{workoutId}/exercises/{exerciseId}/sets/{setId}", cancellationToken);
            
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return Result.NotFound("Подход не найден");
            }
            
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return Result.Failure("Требуется авторизация");
            }

            response.EnsureSuccessStatusCode();
            return Result.Success();
        }
        catch (HttpRequestException ex)
        {
            _logger?.LogError(ex, "HTTP error while deleting set {SetId} from exercise {ExerciseId} in workout {WorkoutId}", setId, exerciseId, workoutId);
            return Result.Failure("Ошибка сети при удалении подхода");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Unexpected error while deleting set {SetId} from exercise {ExerciseId} in workout {WorkoutId}", setId, exerciseId, workoutId);
            return Result.Failure("Неожиданная ошибка при удалении подхода");
        }
    }

    #endregion

    #region ExerciseInfos

    public async Task<Result<IEnumerable<ExerciseInfoDto>>> GetExerciseInfosAsync(string token, CancellationToken cancellationToken = default)
    {
        try
        {
            SetAuthorizationHeader(token);
            _logger?.LogInformation("Getting all exercise infos");
            
            var response = await _httpClient.GetAsync("api/exerciseinfos", cancellationToken);
            
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return Result<IEnumerable<ExerciseInfoDto>>.Failure("Требуется авторизация");
            }

            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var exerciseInfos = JsonSerializer.Deserialize<IEnumerable<ExerciseInfoDto>>(content, _jsonOptions);
            
            return exerciseInfos != null 
                ? Result<IEnumerable<ExerciseInfoDto>>.Success(exerciseInfos)
                : Result<IEnumerable<ExerciseInfoDto>>.Failure("Ошибка десериализации списка информации об упражнениях");
        }
        catch (HttpRequestException ex)
        {
            _logger?.LogError(ex, "HTTP error while getting exercise infos");
            return Result<IEnumerable<ExerciseInfoDto>>.Failure("Ошибка сети при получении информации об упражнениях");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Unexpected error while getting exercise infos");
            return Result<IEnumerable<ExerciseInfoDto>>.Failure("Неожиданная ошибка при получении информации об упражнениях");
        }
    }

    public async Task<Result<ExerciseInfoDto>> GetExerciseInfoByIdAsync(Guid id, string token, CancellationToken cancellationToken = default)
    {
        try
        {
            SetAuthorizationHeader(token);
            _logger?.LogInformation("Getting exercise info by ID: {ExerciseInfoId}", id);
            
            var response = await _httpClient.GetAsync($"api/exerciseinfos/{id}", cancellationToken);
            
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return Result<ExerciseInfoDto>.NotFound("Информация об упражнении не найдена");
            }
            
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return Result<ExerciseInfoDto>.Failure("Требуется авторизация");
            }

            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var exerciseInfo = JsonSerializer.Deserialize<ExerciseInfoDto>(content, _jsonOptions);
            
            return exerciseInfo != null 
                ? Result<ExerciseInfoDto>.Success(exerciseInfo)
                : Result<ExerciseInfoDto>.Failure("Ошибка десериализации информации об упражнении");
        }
        catch (HttpRequestException ex)
        {
            _logger?.LogError(ex, "HTTP error while getting exercise info by ID: {ExerciseInfoId}", id);
            return Result<ExerciseInfoDto>.Failure("Ошибка сети при получении информации об упражнении");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Unexpected error while getting exercise info by ID: {ExerciseInfoId}", id);
            return Result<ExerciseInfoDto>.Failure("Неожиданная ошибка при получении информации об упражнении");
        }
    }

    public async Task<Result> CreateExerciseInfoAsync(ExerciseInfoCreateDto exerciseInfo, string token, CancellationToken cancellationToken = default)
    {
        try
        {
            SetAuthorizationHeader(token);
            _logger?.LogInformation("Creating exercise info '{ExerciseInfoName}'", exerciseInfo.Name);
            
            var response = await _httpClient.PostAsJsonAsync("api/exercise-infos", exerciseInfo, _jsonOptions, cancellationToken);
            
            if (response.StatusCode == HttpStatusCode.Unauthorized) return Result.Failure("Требуется авторизация");
            if (response.StatusCode == HttpStatusCode.Conflict) return Result.Conflict("Упражнение с таким названием уже существует");

            response.EnsureSuccessStatusCode();
            return Result.Success();
        }
        catch (HttpRequestException ex)
        {
            _logger?.LogError(ex, "HTTP error while creating exercise info '{ExerciseInfoName}'", exerciseInfo.Name);
            return Result.Failure("Ошибка сети при создании информации об упражнении");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Unexpected error while creating exercise info '{ExerciseInfoName}'", exerciseInfo.Name);
            return Result.Failure("Неожиданная ошибка при создании информации об упражнении");
        }
    }

    public async Task<Result> UpdateExerciseInfoAsync(Guid id, ExerciseInfoUpdateDto exerciseInfo, string token, CancellationToken cancellationToken = default)
    {
        try
        {
            SetAuthorizationHeader(token);
            _logger?.LogInformation("Updating exercise info: {ExerciseInfoId}", id);
            
            var json = JsonSerializer.Serialize(exerciseInfo, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PutAsync($"api/exerciseinfos/{id}", content, cancellationToken);
            
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return Result.NotFound("Информация об упражнении не найдена");
            }
            
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return Result.Failure("Требуется авторизация");
            }

            response.EnsureSuccessStatusCode();
            return Result.Success();
        }
        catch (HttpRequestException ex)
        {
            _logger?.LogError(ex, "HTTP error while updating exercise info: {ExerciseInfoId}", id);
            return Result.Failure("Ошибка сети при обновлении информации об упражнении");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Unexpected error while updating exercise info: {ExerciseInfoId}", id);
            return Result.Failure("Неожиданная ошибка при обновлении информации об упражнении");
        }
    }

    public async Task<Result> DeleteExerciseInfoAsync(Guid id, string token, CancellationToken cancellationToken = default)
    {
        try
        {
            SetAuthorizationHeader(token);
            _logger?.LogInformation("Deleting exercise info: {ExerciseInfoId}", id);
            
            var response = await _httpClient.DeleteAsync($"api/exerciseinfos/{id}", cancellationToken);
            
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return Result.NotFound("Информация об упражнении не найдена");
            }
            
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return Result.Failure("Требуется авторизация");
            }

            response.EnsureSuccessStatusCode();
            return Result.Success();
        }
        catch (HttpRequestException ex)
        {
            _logger?.LogError(ex, "HTTP error while deleting exercise info: {ExerciseInfoId}", id);
            return Result.Failure("Ошибка сети при удалении информации об упражнении");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Unexpected error while deleting exercise info: {ExerciseInfoId}", id);
            return Result.Failure("Неожиданная ошибка при удалении информации об упражнении");
        }
    }

    #endregion
} 
