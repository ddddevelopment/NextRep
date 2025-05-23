using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Workouts.Api.Models;
using Workouts.Domain.Models;
using Workouts.Domain.Services;

namespace Workouts.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WorkoutsController : ControllerBase
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
        public async Task<ActionResult> Create(WorkoutCreateRequest workoutDto)
        {
            _logger?.LogInformation("Received request to create a new workout: {@WorkoutDto}", workoutDto);
            Workout workout = _mapper.Map<Workout>(workoutDto);

            Result createResult = await _service.Create(workout);

            if (createResult.IsSuccess)
            {
                _logger?.LogInformation("Workout created successfully: {@Workout}", workout);
                return Created();
            }
            else
            {
                switch (createResult.Error?.Type)
                {
                    case ErrorType.Validation:
                        {
                            _logger?.LogWarning(createResult.Error?.Message, "Invalid workout data: {@WorkoutDto}", workoutDto);
                            return BadRequest(createResult.Error?.Message);
                        }
                    case ErrorType.Conflict:
                        {
                            _logger?.LogWarning(createResult.Error?.Message, "Workout already exists: {@WorkoutDto}", workoutDto);
                            return Conflict(createResult.Error?.Message);
                        }
                    case ErrorType.Unknown:
                    default:
                        {
                            _logger?.LogError(createResult.Error?.Message, "An error occurred while creating a workout: {@WorkoutDto}", workoutDto);
                            return BadRequest(createResult.Error?.Message);
                        }
                }
            }
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<WorkoutGetResponse>> GetById(Guid id)
        {
            _logger?.LogInformation("Received request to get workout with ID: {WorkoutId}", id);

            Result<Workout> getResult = await _service.GetById(id);

            if (getResult.IsSuccess)
            {
                var response = _mapper.Map<WorkoutGetResponse>(getResult.Value);
                _logger?.LogInformation("Workout retrieved successfully: {@Workout}", response);
                return Ok(response);
            }
            else
            {
                switch (getResult.Error?.Type)
                {
                    case ErrorType.NotFound:
                        {
                            _logger?.LogWarning(getResult.Error?.Message, "Workout not found with ID: {WorkoutId}", id);
                            return NotFound(getResult.Error?.Message);
                        }
                    case ErrorType.Validation:
                        _logger?.LogWarning(getResult.Error?.Message, "Invalid workout data for get by id: {WorkoutId}", id);
                        return BadRequest(getResult.Error?.Message);
                    case ErrorType.Unknown:
                    default:
                        _logger?.LogError(getResult.Error?.Message, "An error occurred while retrieving workout with ID: {WorkoutId}", id);
                        return BadRequest(getResult.Error?.Message);
                }
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<WorkoutGetResponse>>> GetAll()
        {
            _logger?.LogInformation("Received request to get all workouts");

            Result<IEnumerable<Workout>> getAllResult = await _service.GetAll();

            if (getAllResult.IsSuccess)
            {
                IEnumerable<WorkoutGetResponse> response = _mapper.Map<IEnumerable<WorkoutGetResponse>>(getAllResult.Value);
                _logger?.LogInformation("Successfully retrieved all workouts");
                return Ok(response);
            }
            else
            {
                switch (getAllResult.Error?.Type)
                {
                    case ErrorType.Validation:
                        _logger?.LogWarning(getAllResult.Error?.Message, "Invalid workout data for get all");
                        return BadRequest(getAllResult.Error?.Message);
                    case ErrorType.Unknown:
                    default:
                        _logger?.LogError(getAllResult.Error?.Message, "An error occurred while retrieving all workouts");
                        return BadRequest(getAllResult.Error?.Message);
                }
            }
        }

        [HttpPut]
        public async Task<ActionResult> Update(WorkoutUpdateDto workoutDto)
        {
            _logger?.LogInformation("Received request to update workout: {@WorkoutDto}", workoutDto);

            Workout workout = _mapper.Map<Workout>(workoutDto);
            Result<Workout> result = await _service.Update(workout);

            if (result.IsSuccess)
            {
                _logger?.LogInformation("Workout updated successfully: {@Workout}", workoutDto);
                return Ok();
            }
            else
            {
                switch (result.Error?.Type)
                {
                    case ErrorType.Validation:
                        _logger?.LogWarning(result.Error?.Message, "Invalid workout data for update: {@WorkoutDto}", workoutDto);
                        return BadRequest(result.Error?.Message);
                    case ErrorType.NotFound:
                        _logger?.LogWarning("Workout not found for update: {@WorkoutDto}", workoutDto);
                        return NotFound();
                    case ErrorType.Unknown:
                    default:
                        _logger?.LogError(result.Error?.Message, "An error occurred while updating workout: {@WorkoutDto}", workoutDto);
                        return BadRequest(result.Error?.Message);
                }
            }
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            _logger?.LogInformation("Received request to delete workout with ID: {WorkoutId}", id);

            var result = await _service.Delete(id);

            if (result.IsSuccess)
            {
                _logger?.LogInformation("Workout deleted successfully with ID: {WorkoutId}", id);
                return NoContent();
            }
            else
            {
                switch (result.Error?.Type)
                {
                    case ErrorType.NotFound:
                        _logger?.LogWarning("Workout not found for deletion with ID: {WorkoutId}", id);
                        return NotFound();
                    case ErrorType.Validation:
                        _logger?.LogWarning(result.Error?.Message, "Invalid workout data for deletion: {WorkoutId}", id);
                        return BadRequest(result.Error?.Message);
                    case ErrorType.Unknown:
                    default:
                        _logger?.LogError(result.Error?.Message, "An error occurred while deleting workout with ID: {WorkoutId}", id);
                        return BadRequest(result.Error?.Message);
                }
            }
        }
    }
}
