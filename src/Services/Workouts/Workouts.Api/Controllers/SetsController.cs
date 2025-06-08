using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Workouts.Api.Models;
using Workouts.Domain.Models;
using Workouts.Domain.Services;

namespace Workouts.Api.Controllers;

[ApiController]
[Route("api/workouts/{workoutId:guid}/exercises/{exerciseId:guid}/sets")]
public class SetsController : ControllerBase
{
    private readonly ISetsService _setsService;
    private readonly IMapper _mapper;
    private readonly ILogger<SetsController>? _logger;

    public SetsController(
        ISetsService setsService,
        IMapper mapper,
        ILogger<SetsController>? logger = null)
    {
        _setsService = setsService;
        _mapper = mapper;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult> AddSetToExercise(Guid workoutId, Guid exerciseId, SetCreateRequestForExercise request)
    {
        _logger?.LogInformation("Received request to add set to exercise ID {ExerciseId} in workout {WorkoutId}: {@SetCreateRequest}", exerciseId, workoutId, request);

        Set set = _mapper.Map<Set>(request, opt => opt.AfterMap((src, dest) => dest.ExerciseId = exerciseId));
        Result createSetResult = await _setsService.Create(workoutId, set);

        if (createSetResult.IsSuccess)
        {
            _logger?.LogInformation("Set added successfully to exercise ID {ExerciseId} in workout {WorkoutId}: {@Set}", exerciseId, workoutId, set);
            return Created();
        }

        return HandleErrorResult(createSetResult.Error, request, new { workoutId, exerciseId });
    }

    [HttpGet("{setId:guid}")]
    public async Task<ActionResult<SetDto>> GetSetById(Guid workoutId, Guid exerciseId, Guid setId)
    {
        _logger?.LogInformation("Received request to get set ID {SetId} for exercise ID {ExerciseId} in workout {WorkoutId}", setId, exerciseId, workoutId);

        Result<Set> getResult = await _setsService.GetByIdInExercise(workoutId, exerciseId, setId);

        if (getResult.IsSuccess)
        {
            SetDto response = _mapper.Map<SetDto>(getResult.Value);
            _logger?.LogInformation("Set ID {SetId} for exercise ID {ExerciseId} in workout {WorkoutId} retrieved successfully: {@SetDto}", setId, exerciseId, workoutId, response);
            return Ok(response);
        }
        return HandleErrorResult(getResult.Error, id_param: new { workoutId, exerciseId, setId });
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SetDto>>> GetSetsForExercise(Guid workoutId, Guid exerciseId)
    {
        _logger?.LogInformation("Received request to get sets for exercise ID {ExerciseId} in workout {WorkoutId}", exerciseId, workoutId);

        Result<IEnumerable<Set>> getResult = await _setsService.GetAllByExerciseId(workoutId, exerciseId);

        if (getResult.IsSuccess)
        {
            IEnumerable<SetDto> response = _mapper.Map<IEnumerable<SetDto>>(getResult.Value);
            _logger?.LogInformation("Sets for exercise ID {ExerciseId} in workout {WorkoutId} retrieved successfully", exerciseId, workoutId);
            return Ok(response);
        }

        return HandleErrorResult(getResult.Error, new { workoutId, exerciseId });
    }

    [HttpPut("{setId:guid}")]
    public async Task<ActionResult<SetDto>> UpdateSetInExercise(Guid workoutId, Guid exerciseId, Guid setId, SetUpdateRequest request)
    {
        _logger?.LogInformation("Received request to update set ID {SetId} in exercise ID {ExerciseId} in workout {WorkoutId}: {@SetUpdateRequest}", setId, exerciseId, workoutId, request);

        Set setToUpdate = _mapper.Map<Set>(request, opt => opt.AfterMap((src, dest) => {
            dest.Id = setId;
            dest.ExerciseId = exerciseId;
        }));

        Result<Set> updateResult = await _setsService.Update(workoutId, setToUpdate);

        if (updateResult.IsSuccess)
        {
            _logger?.LogInformation("Set ID {SetId} in exercise ID {ExerciseId} in workout {WorkoutId} updated successfully.", setId, exerciseId, workoutId);
            SetDto setDto = _mapper.Map<SetDto>(updateResult.Value);
            return Ok(setDto);
        }

        return HandleErrorResult(updateResult.Error, request, id_param: new { workoutId, exerciseId, setId });
    }

    [HttpDelete("{setId:guid}")]
    public async Task<ActionResult> DeleteSetFromExercise(Guid workoutId, Guid exerciseId, Guid setId)
    {
        _logger?.LogInformation("Received request to delete set ID {SetId} from exercise ID {ExerciseId} in workout {WorkoutId}.", setId, exerciseId, workoutId);

        Result deleteResult = await _setsService.DeleteFromExercise(workoutId, exerciseId, setId);

        if (deleteResult.IsSuccess)
        {
            _logger?.LogInformation("Set ID {SetId} deleted successfully from exercise ID {ExerciseId} in workout {WorkoutId}.", setId, exerciseId, workoutId);
            return NoContent();
        }
        return HandleErrorResult(deleteResult.Error, id_param: new { workoutId, exerciseId, setId });
    }

    private ActionResult HandleErrorResult(Error? error, object? requestPayload = null, object? id_param = null)
    {
        string logMessage = $"Set Operation Error - Type: {error?.Type}, Message: {error?.Message}, Workout: {id_param}";
        switch (error?.Type)
        {
            case ErrorType.Validation:
                _logger?.LogWarning(logMessage, requestPayload);
                return BadRequest(error?.Message);
            case ErrorType.NotFound:
                _logger?.LogWarning(logMessage, requestPayload);
                return NotFound(error?.Message);
            case ErrorType.Conflict:
                _logger?.LogWarning(logMessage, requestPayload);
                return Conflict(error?.Message);
            case ErrorType.Unknown:
            default:
                _logger?.LogError(logMessage, requestPayload);
                return BadRequest(error?.Message); 
        }
    }
}
