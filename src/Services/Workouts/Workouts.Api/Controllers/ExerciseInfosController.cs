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

        var exerciseInfo = _mapper.Map<ExerciseInfo>(request);

        Result createResult = await _service.Create(exerciseInfo);

        if (createResult.IsSuccess)
        {
            _logger?.LogInformation("ExerciseInfo created successfully: {@ExerciseInfoRequest}", request);
            return Created();
        }
        else
        {
            switch (createResult.Error?.Type)
            {
                case ErrorType.Validation:
                    _logger?.LogWarning(createResult.Error?.Message, "Invalid exerciseInfo data: {@ExerciseInfoRequest}", request);
                    return BadRequest(createResult.Error?.Message);
                case ErrorType.Conflict:
                    _logger?.LogWarning(createResult.Error?.Message, "ExerciseInfo already exists: {@ExerciseInfoRequest}", request);
                    return Conflict(createResult.Error?.Message);
                case ErrorType.Unknown:
                default:
                    _logger?.LogError(createResult.Error?.Message, "An error occurred while creating an exerciseInfo: {@ExerciseInfoRequest}", request);
                    return BadRequest(createResult.Error?.Message);
            }
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ExerciseInfoDto>> GetById(Guid id)
    {
        _logger?.LogInformation("Received request to get exerciseInfo with ID: {ExerciseInfoId}", id);

        var getResult = await _service.GetById(id);

        if (getResult.IsSuccess)
        {
            var response = _mapper.Map<ExerciseInfoDto>(getResult.Value);
            _logger?.LogInformation("ExerciseInfo retrieved successfully: {@ExerciseInfo}", response);
            return Ok(response);
        }
        else
        {
            switch (getResult.Error?.Type)
            {
                case ErrorType.NotFound:
                    _logger?.LogWarning(getResult.Error?.Message, "ExerciseInfo not found with ID: {ExerciseInfoId}", id);
                    return NotFound(getResult.Error?.Message);
                case ErrorType.Validation:
                    _logger?.LogWarning(getResult.Error?.Message, "Invalid exerciseInfo data for get by id: {ExerciseInfoId}", id);
                    return BadRequest(getResult.Error?.Message);
                case ErrorType.Unknown:
                default:
                    _logger?.LogError(getResult.Error?.Message, "An error occurred while retrieving exerciseInfo with ID: {ExerciseInfoId}", id);
                    return BadRequest(getResult.Error?.Message);
            }
        }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ExerciseInfoDto>>> GetAll()
    {
        _logger?.LogInformation("Received request to get all exerciseInfos");

        var getAllResult = await _service.GetAll();

        if (getAllResult.IsSuccess)
        {
            var response = _mapper.Map<IEnumerable<ExerciseInfoDto>>(getAllResult.Value);
            _logger?.LogInformation("Successfully retrieved all exerciseInfos");
            return Ok(response);
        }
        else
        {
            switch (getAllResult.Error?.Type)
            {
                case ErrorType.Unknown:
                default:
                    _logger?.LogError(getAllResult.Error?.Message, "An error occurred while retrieving all exerciseInfos");
                    return BadRequest(getAllResult.Error?.Message);
            }
        }
    }

    [HttpPut]
    public async Task<ActionResult> Update(ExerciseInfoDto exerciseInfoDto)
    {
        _logger?.LogInformation("Received request to update exerciseInfo: {@ExerciseInfoDto}", exerciseInfoDto);

        var exerciseInfo = _mapper.Map<ExerciseInfo>(exerciseInfoDto);
        var result = await _service.Update(exerciseInfo);

        if (result.IsSuccess)
        {
            _logger?.LogInformation("ExerciseInfo updated successfully: {@ExerciseInfoDto}", exerciseInfoDto);
            return Ok();
        }
        else
        {
            switch (result.Error?.Type)
            {
                case ErrorType.Validation:
                    _logger?.LogWarning(result.Error?.Message, "Invalid exerciseInfo data for update: {@ExerciseInfoDto}", exerciseInfoDto);
                    return BadRequest(result.Error?.Message);
                case ErrorType.NotFound:
                    _logger?.LogWarning("ExerciseInfo not found for update: {@ExerciseInfoDto}", exerciseInfoDto);
                    return NotFound();
                case ErrorType.Unknown:
                default:
                    _logger?.LogError(result.Error?.Message, "An error occurred while updating exerciseInfo: {@ExerciseInfoDto}", exerciseInfoDto);
                    return BadRequest(result.Error?.Message);
            }
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        _logger?.LogInformation("Received request to delete exerciseInfo with ID: {ExerciseInfoId}", id);

        var result = await _service.Delete(id);

        if (result.IsSuccess)
        {
            _logger?.LogInformation("ExerciseInfo deleted successfully with ID: {ExerciseInfoId}", id);
            return NoContent();
        }
        else
        {
            switch (result.Error?.Type)
            {
                case ErrorType.NotFound:
                    _logger?.LogWarning("ExerciseInfo not found for deletion with ID: {ExerciseInfoId}", id);
                    return NotFound();
                case ErrorType.Validation:
                    _logger?.LogWarning(result.Error?.Message, "Invalid exerciseInfo data for deletion: {ExerciseInfoId}", id);
                    return BadRequest(result.Error?.Message);
                case ErrorType.Unknown:
                default:
                    _logger?.LogError(result.Error?.Message, "An error occurred while deleting exerciseInfo with ID: {ExerciseInfoId}", id);
                    return BadRequest(result.Error?.Message);
            }
        }
    }
}