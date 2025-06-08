using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Workouts.Api.Models;
using Workouts.Domain.Models;
using Workouts.Domain.Services;

namespace Workouts.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExerciseInfosController : ControllerBase
{
    private readonly IExerciseInfosService _service;
    private readonly IMapper _mapper;
    private readonly ILogger<ExerciseInfosController>? _logger;

    public ExerciseInfosController(IExerciseInfosService service, IMapper mapper, ILogger<ExerciseInfosController>? logger = null)
    {
        _service = service;
        _mapper = mapper;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult> Create(ExerciseInfoCreateRequest request)
    {
        _logger?.LogInformation("Received request to create a new exerciseInfo: {@ExerciseInfoRequest}", request);

        ExerciseInfo exerciseInfo = _mapper.Map<ExerciseInfo>(request);
        Result createResult = await _service.Create(exerciseInfo);

        if (createResult.IsSuccess)
        {
            _logger?.LogInformation("ExerciseInfo created successfully: {@ExerciseInfoRequest}", request);
            return Created();
        }
        
        return HandleErrorResult(createResult.Error, request);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ExerciseInfoDto>> GetById(Guid id)
    {
        _logger?.LogInformation("Received request to get exerciseInfo with ID: {ExerciseInfoId}", id);
        Result<ExerciseInfo> getResult = await _service.GetById(id);

        if (getResult.IsSuccess)
        {
            ExerciseInfoDto response = _mapper.Map<ExerciseInfoDto>(getResult.Value);
            _logger?.LogInformation("ExerciseInfo retrieved successfully: {@ExerciseInfo}", response);
            return Ok(response);
        }
        
        return HandleErrorResult(getResult.Error, id_param: id);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ExerciseInfoDto>>> GetAll()
    {
        _logger?.LogInformation("Received request to get all exerciseInfos");
        Result<IEnumerable<ExerciseInfo>> getAllResult = await _service.GetAll();

        if (getAllResult.IsSuccess)
        {
            IEnumerable<ExerciseInfoDto> response = _mapper.Map<IEnumerable<ExerciseInfoDto>>(getAllResult.Value);
            _logger?.LogInformation("Successfully retrieved all exerciseInfos");
            return Ok(response);
        }
        
        return HandleErrorResult(getAllResult.Error);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult> Update(Guid id, ExerciseInfoUpdateRequest request)
    {
        _logger?.LogInformation("Received request to update exerciseInfo: {@ExerciseInfoUpdateRequest}", request);

        ExerciseInfo exerciseInfo = _mapper.Map<ExerciseInfo>(request, opt => opt.AfterMap((src, dest) => dest.Id = id));
        Result<ExerciseInfo> result = await _service.Update(exerciseInfo);

        if (result.IsSuccess)
        {
            _logger?.LogInformation("ExerciseInfo updated successfully: {@ExerciseInfoDto}", request);
            return Ok();
        }
        
        return HandleErrorResult(result.Error, request);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        _logger?.LogInformation("Received request to delete exerciseInfo with ID: {ExerciseInfoId}", id);
        Result result = await _service.Delete(id);

        if (result.IsSuccess)
        {
            _logger?.LogInformation("ExerciseInfo deleted successfully with ID: {ExerciseInfoId}", id);
            return NoContent();
        }
        
        return HandleErrorResult(result.Error, id_param: id);
    }

    private ActionResult HandleErrorResult(Error? error, object? requestPayload = null, object? id_param = null)
    {
        string logMessage = $"ExerciseInfo Operation Error - Type: {error?.Type}, Message: {error?.Message}";
        if (id_param != null) 
        {
            logMessage += $", ID: {id_param}";
        }

        switch (error?.Type)
        {
            case ErrorType.Validation:
                _logger?.LogWarning(logMessage, "Invalid exercise info data: {@RequestPayload}", requestPayload);
                return BadRequest(error?.Message);
            case ErrorType.NotFound:
                _logger?.LogWarning(logMessage, "Exercise info resource not found: {@RequestPayload}", requestPayload);
                return NotFound(error?.Message);
            case ErrorType.Conflict:
                _logger?.LogWarning(logMessage, "Exercise info conflict occurred: {@RequestPayload}", requestPayload);
                return Conflict(error?.Message);
            case ErrorType.Unknown:
            default:
                _logger?.LogError(logMessage, "An unknown error occurred during exercise info operation: {@RequestPayload}", requestPayload);
                return BadRequest(error?.Message);
        }
    }
}