using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Workouts.Api.Models;
using Workouts.Domain.Models;
using Workouts.Domain.Services;

namespace Workouts.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WorkoutsController : AuthorizedControllerBase
{
    private readonly IWorkoutsService _service;
    private readonly IMapper _mapper;
    private readonly ILogger<WorkoutsController>? _logger;

    public WorkoutsController(IWorkoutsService service, IMapper mapper, ILogger<WorkoutsController>? logger = null)
    {
        _service = service;
        _mapper = mapper;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult> Create(WorkoutCreateRequest request)
    {
        _logger?.LogInformation("Received request to create a new workout: {@WorkoutCreateRequest}", request);

        Workout workout = _mapper.Map<Workout>(request, opt => opt.AfterMap((src, dest) => dest.UserId = CurrentUserId));
        Result createResult = await _service.Create(workout);

        if (createResult.IsSuccess)
        {
            _logger?.LogInformation("Workout created successfully: {@Workout}", request);
            return Created();
        }

        return HandleWorkoutErrorResult(createResult.Error, request);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<WorkoutDto>> GetById(Guid id)
    {
        _logger?.LogInformation("Received request to get workout with ID: {WorkoutId}", id);
        Result<Workout> getResult = await _service.GetById(id);

        if (getResult.IsSuccess)
        {
            WorkoutDto response = _mapper.Map<WorkoutDto>(getResult.Value);
            _logger?.LogInformation("Workout retrieved successfully: {@Workout}", response);
            return Ok(response);
        }

        return HandleWorkoutErrorResult(getResult.Error, id_param: id);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<WorkoutDto>>> GetAll()
    {
        _logger?.LogInformation("Received request to get all workouts");
        Result<IEnumerable<Workout>> getAllResult = await _service.GetAll();

        if (getAllResult.IsSuccess)
        {
            IEnumerable<WorkoutDto> response = _mapper.Map<IEnumerable<WorkoutDto>>(getAllResult.Value);
            _logger?.LogInformation("Successfully retrieved all workouts");
            return Ok(response);
        }

        return HandleWorkoutErrorResult(getAllResult.Error);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult> Update(Guid id, WorkoutUpdateRequest request)
    {
        _logger?.LogInformation("Received request to update workout: {@WorkoutUpdateRequest}", request);
        Workout workout = _mapper.Map<Workout>(request, opt =>
        {
            opt.AfterMap((src, dest) =>
            {
                dest.Id = id;
                dest.UserId = CurrentUserId;
            });
        });
        Result<Workout> result = await _service.Update(workout);

        if (result.IsSuccess)
        {
            _logger?.LogInformation("Workout updated successfully: {@Workout}", request);
            return Ok();
        }

        return HandleWorkoutErrorResult(result.Error, request);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        _logger?.LogInformation("Received request to delete workout with ID: {WorkoutId}", id);
        Result result = await _service.Delete(id);

        if (result.IsSuccess)
        {
            _logger?.LogInformation("Workout deleted successfully with ID: {WorkoutId}", id);
            return NoContent();
        }

        return HandleWorkoutErrorResult(result.Error, id_param: id);
    }

    private ActionResult HandleWorkoutErrorResult(Error? error, object? requestPayload = null, object? id_param = null)
    {
        string logMessage = $"Workout Operation Error - Type: {error?.Type}, Message: {error?.Message}";
        if (id_param != null)
        {
            logMessage += $", ID: {id_param}";
        }

        switch (error?.Type)
        {
            case ErrorType.Validation:
                _logger?.LogWarning(logMessage, "Invalid workout data: {@RequestPayload}", requestPayload);
                return BadRequest(error?.Message);
            case ErrorType.NotFound:
                _logger?.LogWarning(logMessage, "Workout resource not found: {@RequestPayload}", requestPayload);
                return NotFound(error?.Message);
            case ErrorType.Conflict:
                _logger?.LogWarning(logMessage, "Workout conflict occurred: {@RequestPayload}", requestPayload);
                return Conflict(error?.Message);
            case ErrorType.Unknown:
            default:
                _logger?.LogError(logMessage, "An unknown error occurred during workout operation: {@RequestPayload}", requestPayload);
                return BadRequest(error?.Message);
        }
    }
}
