using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Workouts.Api.Models;
using Workouts.Domain.Models;
using Workouts.Domain.Services;

namespace Workouts.Api.Controllers;

[ApiController]
[Route("api/exercises/{exerciseId:guid}/sets")]
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
    public async Task<ActionResult> AddSetToExercise(Guid exerciseId, SetCreateRequestForExercise request)
    {
        _logger?.LogInformation("Received request to add set to exercise ID {ExerciseId}: {@SetCreateRequest}", exerciseId, request);

        Set set = _mapper.Map<Set>(request, opt => opt.AfterMap((src, dest) => dest.ExerciseId = exerciseId));
        Result createSetResult = await _setsService.Create(set);

        if (createSetResult.IsSuccess)
        {
            _logger?.LogInformation("Set added successfully to exercise ID {ExerciseId}: {@Set}", exerciseId, set);
            return Created();
        }

        return HandleErrorResult(createSetResult.Error, request, exerciseId);
    }

    [HttpGet("{setId:guid}")]
    public async Task<ActionResult<SetDto>> GetSetById(Guid exerciseId, Guid setId)
    {
        _logger?.LogInformation("Received request to get set ID {SetId} for exercise ID: {ExerciseId}", setId, exerciseId);

        Result<Set> getResult = await _setsService.GetByIdInExercise(exerciseId, setId);

        if (getResult.IsSuccess)
        {
            SetDto response = _mapper.Map<SetDto>(getResult.Value);
            _logger?.LogInformation("Set ID {SetId} for exercise ID {ExerciseId} retrieved successfully: {@SetDto}", setId, exerciseId, response);
            return Ok(response);
        }
        return HandleErrorResult(getResult.Error, id_param: new { exerciseId, setId });
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SetDto>>> GetSetsForExercise(Guid exerciseId)
    {
        _logger?.LogInformation("Received request to get sets for exercise ID: {ExerciseId}", exerciseId);

        Result<IEnumerable<Set>> getResult = await _setsService.GetAllByExerciseId(exerciseId);

        if (getResult.IsSuccess)
        {
            IEnumerable<SetDto> response = _mapper.Map<IEnumerable<SetDto>>(getResult.Value);
            _logger?.LogInformation("Sets for exercise ID {ExerciseId} retrieved successfully", exerciseId);
            return Ok(response);
        }

        return HandleErrorResult(getResult.Error, new { ExerciseId = exerciseId });
    }

    [HttpPut("{setId:guid}")]
    public async Task<ActionResult<SetDto>> UpdateSetInExercise(Guid exerciseId, Guid setId, SetUpdateRequest request)
    {
        _logger?.LogInformation("Received request to update set ID {SetId} in exercise ID {ExerciseId}: {@SetUpdateRequest}", setId, exerciseId, request);

        Set setToUpdate = _mapper.Map<Set>(request, opt => opt.AfterMap((src, dest) => {
            dest.Id = setId;
            dest.ExerciseId = exerciseId;
        }));

        Result<Set> updateResult = await _setsService.Update(setToUpdate);

        if (updateResult.IsSuccess)
        {
            _logger?.LogInformation("Set ID {SetId} in exercise ID {ExerciseId} updated successfully.", setId, exerciseId);
            SetDto setDto = _mapper.Map<SetDto>(updateResult.Value);
            return Ok(setDto);
        }

        return HandleErrorResult(updateResult.Error, request, id_param: new { exerciseId, setId });
    }

    [HttpDelete("{setId:guid}")]
    public async Task<ActionResult> DeleteSetFromExercise(Guid exerciseId, Guid setId)
    {
        _logger?.LogInformation("Received request to delete set ID {SetId} from exercise ID {ExerciseId}.", setId, exerciseId);

        Result deleteResult = await _setsService.DeleteFromExercise(exerciseId, setId);

        if (deleteResult.IsSuccess)
        {
            _logger?.LogInformation("Set ID {SetId} deleted successfully from exercise ID {ExerciseId}.", setId, exerciseId);
            return NoContent();
        }
        return HandleErrorResult(deleteResult.Error, id_param: new { exerciseId, setId });
    }

    private ActionResult HandleErrorResult(Error? error, object? requestPayload = null, object? id_param = null)
    {
        string logMessage = $"Set Operation Error - Type: {error?.Type}, Message: {error?.Message}";
        if (id_param != null) 
        {
            logMessage += $", ID: {id_param}";
        }

        switch (error?.Type)
        {
            case ErrorType.Validation:
                _logger?.LogWarning(
                    "Set Operation Error - Type: {ErrorType}, Message: {ErrorMessage}, Invalid set data: {@RequestPayload}, ID: {IdParam}",
                    error?.Type, error?.Message, requestPayload, id_param);
                return BadRequest(error?.Message);
            case ErrorType.NotFound:
                _logger?.LogWarning(
                    "Set Operation Error - Type: {ErrorType}, Message: {ErrorMessage}, Set resource not found: {@RequestPayload}, ID: {IdParam}",
                    error?.Type, error?.Message, requestPayload, id_param);
                return NotFound(error?.Message);
            case ErrorType.Conflict:
                _logger?.LogWarning(
                    "Set Operation Error - Type: {ErrorType}, Message: {ErrorMessage}, Set conflict occurred: {@RequestPayload}, ID: {IdParam}",
                    error?.Type, error?.Message, requestPayload, id_param);
                return Conflict(error?.Message);
            case ErrorType.Unknown:
            default:
                _logger?.LogError(
                    "Set Operation Error - Type: {ErrorType}, Message: {ErrorMessage}, An unknown error occurred during set operation: {@RequestPayload}, ID: {IdParam}",
                    error?.Type, error?.Message, requestPayload, id_param);
                return BadRequest(error?.Message); 
        }
    }
}
