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
    private readonly IWorkoutsService _workoutsService;
    private readonly IMapper _mapper;
    private readonly ILogger<ExercisesController>? _logger;

    public ExercisesController(
        IExercisesService exercisesService,
        IWorkoutsService workoutsService,
        IMapper mapper,
        ILogger<ExercisesController>? logger = null)
    {
        _exercisesService = exercisesService;
        _workoutsService = workoutsService;
        _mapper = mapper;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult> AddExerciseToWorkout(Guid workoutId, ExerciseCreateRequestForWorkout request)
    {
        _logger?.LogInformation("Received request to add exercise to workout ID {WorkoutId}: {@ExerciseCreateRequest}", workoutId, request);

        Result<Workout> workoutExistsResult = await _workoutsService.GetById(workoutId);
        if (workoutExistsResult.IsSuccess == false)
        {
            _logger?.LogWarning("Workout with ID {WorkoutId} not found when trying to add exercise.", workoutId);
            return HandleErrorResult(new Error(ErrorType.NotFound, $"Workout with ID {workoutId} not found."), id_param: workoutId);
        }

        Exercise exercise = _mapper.Map<Exercise>(request, opt => opt.AfterMap((src, dest) => dest.WorkoutId = workoutId));

        Result createExerciseResult = await _exercisesService.Create(exercise);

        if (createExerciseResult.IsSuccess)
        {
            _logger?.LogInformation("Exercise added successfully to workout ID {WorkoutId}: {@Exercise}", workoutId, exercise);
            return Created();
        }

        return HandleErrorResult(createExerciseResult.Error, request, workoutId);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ExerciseDto>>> GetExercisesForWorkout(Guid workoutId)
    {
        _logger?.LogInformation("Received request to get exercises for workout ID: {WorkoutId}", workoutId);

        Result<Workout> workoutExistsResult = await _workoutsService.GetById(workoutId);
        if (workoutExistsResult.IsSuccess == false)
        {
            _logger?.LogWarning("Workout with ID {WorkoutId} not found when trying to get exercises.", workoutId);
            return HandleErrorResult(new Error(ErrorType.NotFound, $"Workout with ID {workoutId} not found."), id_param: workoutId);
        }

        Result<IEnumerable<Exercise>> getResult = await _exercisesService.GetByWorkoutId(workoutId);

        if (getResult.IsSuccess)
        {
            IEnumerable<ExerciseDto> response = _mapper.Map<IEnumerable<ExerciseDto>>(getResult.Value);
            _logger?.LogInformation("Exercises for workout ID {WorkoutId} retrieved successfully", workoutId);
            return Ok(response);
        }

        return HandleErrorResult(getResult.Error, new { WorkoutId = workoutId });
    }

    [HttpGet("{exerciseId:guid}")]
    public async Task<ActionResult<ExerciseDto>> GetExerciseById(Guid workoutId, Guid exerciseId)
    {
        _logger?.LogInformation("Received request to get exercise ID {ExerciseId} for workout ID: {WorkoutId}", exerciseId, workoutId);

        Result<Workout> workoutExistsResult = await _workoutsService.GetById(workoutId);
        if (workoutExistsResult.IsSuccess == false)
        {
            _logger?.LogWarning("Workout ID {WorkoutId} not found when getting exercise ID {ExerciseId}.", workoutId, exerciseId);
            return HandleErrorResult(new Error(ErrorType.NotFound, $"Workout with ID {workoutId} not found."), id_param: workoutId);
        }

        Result<Exercise> getResult = await _exercisesService.GetById(exerciseId);

        if (getResult.IsSuccess)
        {
            if (getResult.Value!.WorkoutId != workoutId)
            {
                _logger?.LogWarning("Exercise ID {ExerciseId} found, but it does not belong to workout ID {WorkoutId}.", exerciseId, workoutId);
                return HandleErrorResult(new Error(ErrorType.NotFound, $"Exercise with ID {exerciseId} not found in workout {workoutId}."),
                                         requestPayload: new { workoutId, exerciseId });
            }
            ExerciseDto response = _mapper.Map<ExerciseDto>(getResult.Value);
            _logger?.LogInformation("Exercise ID {ExerciseId} for workout ID {WorkoutId} retrieved successfully: {@ExerciseDto}", exerciseId, workoutId, response);
            return Ok(response);
        }
        return HandleErrorResult(getResult.Error, id_param: new { workoutId, exerciseId });
    }

    [HttpPut("{exerciseId:guid}")]
    public async Task<ActionResult> UpdateExerciseInWorkout(Guid workoutId, Guid exerciseId, ExerciseUpdateRequest request)
    {
        _logger?.LogInformation("Received request to update exercise ID {ExerciseId} in workout ID {WorkoutId}: {@ExerciseUpdateRequest}", exerciseId, workoutId, request);

        Result<Workout> workoutExistsResult = await _workoutsService.GetById(workoutId);
        if (workoutExistsResult.IsSuccess == false)
        {
            _logger?.LogWarning("Workout ID {WorkoutId} not found when updating exercise ID {ExerciseId}.", workoutId, exerciseId);
            return HandleErrorResult(new Error(ErrorType.NotFound, $"Workout with ID {workoutId} not found."), id_param: workoutId);
        }

        Result<Exercise> currentExerciseResult = await _exercisesService.GetById(exerciseId);
        if (currentExerciseResult.IsSuccess == false)
        {
            _logger?.LogWarning("Exercise ID {ExerciseId} not found for update in workout ID {WorkoutId}. Result: {ErrorMessage}", exerciseId, workoutId, currentExerciseResult.Error?.Message);
            return HandleErrorResult(currentExerciseResult.Error, requestPayload: request, id_param: new { workoutId, exerciseId });
        }

        if (currentExerciseResult.Value!.WorkoutId != workoutId)
        {
            _logger?.LogWarning("Attempt to update exercise ID {ExerciseId} which does not belong to workout ID {WorkoutId}.", exerciseId, workoutId);
            return HandleErrorResult(new Error(ErrorType.Validation, $"Exercise {exerciseId} does not belong to workout {workoutId}."),
                                     requestPayload: request, id_param: new { workoutId, exerciseId });
        }

        Exercise exerciseToUpdate = _mapper.Map<Exercise>(request);
        exerciseToUpdate.Id = exerciseId;
        exerciseToUpdate.WorkoutId = workoutId;

        Result<Exercise> updateResult = await _exercisesService.Update(exerciseToUpdate);

        if (updateResult.IsSuccess)
        {
            _logger?.LogInformation("Exercise ID {ExerciseId} in workout ID {WorkoutId} updated successfully.", exerciseId, workoutId);
            return Ok();
        }
        return HandleErrorResult(updateResult.Error, request, id_param: new { workoutId, exerciseId });
    }

    [HttpDelete("{exerciseId:guid}")]
    public async Task<ActionResult> DeleteExerciseFromWorkout(Guid workoutId, Guid exerciseId)
    {
        _logger?.LogInformation("Received request to delete exercise ID {ExerciseId} from workout ID {WorkoutId}.", exerciseId, workoutId);

        Result<Workout> workoutExistsResult = await _workoutsService.GetById(workoutId);
        if (workoutExistsResult.IsSuccess == false)
        {
            _logger?.LogWarning("Workout ID {WorkoutId} not found when deleting exercise ID {ExerciseId}.", workoutId, exerciseId);
            return HandleErrorResult(new Error(ErrorType.NotFound, $"Workout with ID {workoutId} not found."), id_param: workoutId);
        }

        Result<Exercise> currentExerciseResult = await _exercisesService.GetById(exerciseId);
        if (currentExerciseResult.IsSuccess == false)
        {
            _logger?.LogWarning("Exercise ID {ExerciseId} not found for deletion in workout ID {WorkoutId}. Result: {ErrorMessage}", exerciseId, workoutId, currentExerciseResult.Error?.Message);
            return HandleErrorResult(currentExerciseResult.Error, id_param: new { workoutId, exerciseId });
        }

        if (currentExerciseResult.Value!.WorkoutId != workoutId)
        {
            _logger?.LogWarning("Attempt to delete exercise ID {ExerciseId} which does not belong to workout ID {WorkoutId}.", exerciseId, workoutId);
            return HandleErrorResult(new Error(ErrorType.NotFound, $"Exercise {exerciseId} does not belong to workout {workoutId}."),
                                     id_param: new { workoutId, exerciseId });
        }

        Result deleteResult = await _exercisesService.Delete(exerciseId);

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