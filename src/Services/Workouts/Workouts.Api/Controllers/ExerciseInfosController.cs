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
    private readonly IExerciseInfosService _exerciseInfosService;
    private readonly IMapper _mapper;
    private readonly ILogger<ExerciseInfosController>? _logger;

    public ExerciseInfosController(IExerciseInfosService exerciseInfosService, IMapper mapper, ILogger<ExerciseInfosController>? logger = null)
    {
        _exerciseInfosService = exerciseInfosService;
        _mapper = mapper;
        _logger = logger;   
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ExerciseInfoRequest request)
    {
        var exerciseInfo = _mapper.Map<ExerciseInfo>(request);
        var result = await _exerciseInfosService.Create(exerciseInfo);
        return result.IsSuccess ? CreatedAtAction(nameof(GetById), new { id = exerciseInfo.Id }, exerciseInfo) : BadRequest(result.Error);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _exerciseInfosService.GetAll();
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _exerciseInfosService.GetById(id);
        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] ExerciseInfo exerciseInfo)
    {
        if (id != exerciseInfo.Id)
            return BadRequest("ID mismatch");

        var result = await _exerciseInfosService.Update(exerciseInfo);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _exerciseInfosService.Delete(id);
        return result.IsSuccess ? NoContent() : NotFound(result.Error);
    }
}