using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Workouts.Api.Models;
using Workouts.Domain.Models;
using Workouts.Domain.Services;

namespace Workouts.Api.Controllers;

[ApiController]
[Route("api/workouts/{workoutId:guid}/exercises")]
public class ExercisesController : ControllerBase
{
    private readonly IExercisesService _exercisesService;
    private readonly IMapper _mapper;
    private readonly ILogger<ExercisesController>? _logger;

    public ExercisesController(
        IExercisesService exercisesService,
        IMapper mapper,
        ILogger<ExercisesController>? logger = null)
    {
        _exercisesService = exercisesService;
        _mapper = mapper;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult> AddExerciseToWorkout(Guid workoutId, ExerciseCreateRequestForWorkout request)
    {
        _logger?.LogInformation("Received request to add exercise to workout ID {WorkoutId}: {@ExerciseCreateRequest}", workoutId, request);

        Exercise exercise = _mapper.Map<Exercise>(request, opt => opt.AfterMap((src, dest) => dest.WorkoutId = workoutId));
        Result createExerciseResult = await _exercisesService.Create(exercise);

        if (createExerciseResult.IsSuccess)
        {
            _logger?.LogInformation("Exercise added successfully to workout ID {WorkoutId}: {@Exercise}", workoutId, exercise);
            return Created();
        }

        return HandleErrorResult(createExerciseResult.Error, request, workoutId);
    }

    [HttpGet("{exerciseId:guid}")]
    public async Task<ActionResult<ExerciseDto>> GetExerciseById(Guid workoutId, Guid exerciseId)
    {
        _logger?.LogInformation("Received request to get exercise ID {ExerciseId} for workout ID: {WorkoutId}", exerciseId, workoutId);

        Result<Exercise> getResult = await _exercisesService.GetByIdInWorkout(workoutId, exerciseId);

        if (getResult.IsSuccess)
        {
            ExerciseDto response = _mapper.Map<ExerciseDto>(getResult.Value);
            _logger?.LogInformation("Exercise ID {ExerciseId} for workout ID {WorkoutId} retrieved successfully: {@ExerciseDto}", exerciseId, workoutId, response);
            return Ok(response);
        }
        return HandleErrorResult(getResult.Error, id_param: new { workoutId, exerciseId });
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ExerciseDto>>> GetExercisesForWorkout(Guid workoutId)
    {
        _logger?.LogInformation("Received request to get exercises for workout ID: {WorkoutId}", workoutId);

        Result<IEnumerable<Exercise>> getResult = await _exercisesService.GetAllByWorkoutId(workoutId);

        if (getResult.IsSuccess)
        {
            IEnumerable<ExerciseDto> response = _mapper.Map<IEnumerable<ExerciseDto>>(getResult.Value);
            _logger?.LogInformation("Exercises for workout ID {WorkoutId} retrieved successfully", workoutId);
            return Ok(response);
        }

        return HandleErrorResult(getResult.Error, new { WorkoutId = workoutId });
    }

    [HttpPut("{exerciseId:guid}")]
    public async Task<ActionResult<ExerciseDto>> UpdateExerciseInWorkout(Guid workoutId, Guid exerciseId, ExerciseUpdateRequest request)
    {
        _logger?.LogInformation("Received request to update exercise ID {ExerciseId} in workout ID {WorkoutId}: {@ExerciseUpdateRequest}", exerciseId, workoutId, request);

        Exercise exerciseToUpdate = _mapper.Map<Exercise>(request, opt => opt.AfterMap((src, dest) => {
            dest.Id = exerciseId;
            dest.WorkoutId = workoutId;
        }));

        Result<Exercise> updateResult = await _exercisesService.Update(exerciseToUpdate);

        if (updateResult.IsSuccess)
        {
            _logger?.LogInformation("Exercise ID {ExerciseId} in workout ID {WorkoutId} updated successfully.", exerciseId, workoutId);
            ExerciseDto exerciseDto = _mapper.Map<ExerciseDto>(updateResult.Value);
            return Ok(exerciseDto);
        }

        return HandleErrorResult(updateResult.Error, request, id_param: new { workoutId, exerciseId });
    }

    [HttpDelete("{exerciseId:guid}")]
    public async Task<ActionResult> DeleteExerciseFromWorkout(Guid workoutId, Guid exerciseId)
    {
        _logger?.LogInformation("Received request to delete exercise ID {ExerciseId} from workout ID {WorkoutId}.", exerciseId, workoutId);

        Result deleteResult = await _exercisesService.DeleteFromWorkout(workoutId, exerciseId);

        if (deleteResult.IsSuccess)
        {
            _logger?.LogInformation("Exercise ID {ExerciseId} deleted successfully from workout ID {WorkoutId}.", exerciseId, workoutId);
            return NoContent();
        }
        return HandleErrorResult(deleteResult.Error, id_param: new { workoutId, exerciseId });
    }

    private ActionResult HandleErrorResult(Error? error, object? requestPayload = null, object? id_param = null)
    {
        string logMessage = $"Exercise Operation Error - Type: {error?.Type}, Message: {error?.Message}";
        if (id_param != null) 
        {
            logMessage += $", ID: {id_param}";
        }

        switch (error?.Type)
        {
            case ErrorType.Validation:
                _logger?.LogWarning(
                    "Exercise Operation Error - Type: {ErrorType}, Message: {ErrorMessage}, Invalid exercise data: {@RequestPayload}, ID: {IdParam}",
                    error?.Type, error?.Message, requestPayload, id_param);
                return BadRequest(error?.Message);
            case ErrorType.NotFound:
                _logger?.LogWarning(
                    "Exercise Operation Error - Type: {ErrorType}, Message: {ErrorMessage}, Exercise resource not found: {@RequestPayload}, ID: {IdParam}",
                    error?.Type, error?.Message, requestPayload, id_param);
                return NotFound(error?.Message);
            case ErrorType.Conflict:
                _logger?.LogWarning(
                    "Exercise Operation Error - Type: {ErrorType}, Message: {ErrorMessage}, Exercise conflict occurred: {@RequestPayload}, ID: {IdParam}",
                    error?.Type, error?.Message, requestPayload, id_param);
                return Conflict(error?.Message);
            case ErrorType.Unknown:
            default:
                _logger?.LogError(
                    "Exercise Operation Error - Type: {ErrorType}, Message: {ErrorMessage}, An unknown error occurred during exercise operation: {@RequestPayload}, ID: {IdParam}",
                    error?.Type, error?.Message, requestPayload, id_param);
                return BadRequest(error?.Message); 
        }
    }
}