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

        public WorkoutsController(IWorkoutsService service, IMapper mapper, ILogger<WorkoutsController> logger = null)
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

            var createResult = await _service.Create(workout);

            if (createResult.IsSuccess)
            {
                _logger?.LogInformation("Workout created successfully: {@Workout}", workout);
                return Created();
            }
            else
            {
                return BadRequest(createResult.Error.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<WorkoutGetResponse>> GetById(Guid id)
        {
            var result = await _service.GetById(id);
            if (result.IsSuccess)
                return Ok(_mapper.Map<WorkoutGetResponse>(result.Value));
            return NotFound(result.Error.Message);
        }

        [HttpPut]
        public async Task<ActionResult> Update(WorkoutUpdateDto workoutDto)
        {
            var workout = _mapper.Map<Workout>(workoutDto);
            var result = await _service.Update(workout);
            if (result.IsSuccess)
                return NoContent();
            return BadRequest(result.Error.Message);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var result = await _service.Delete(id);
            if (result.IsSuccess)
                return NoContent();
            return NotFound(result.Error.Message);
        }
    }
}
