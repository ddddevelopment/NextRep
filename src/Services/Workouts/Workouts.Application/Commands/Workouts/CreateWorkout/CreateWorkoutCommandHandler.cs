using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Workouts.Domain.Models;
using Workouts.Domain.Repositories;

namespace Workouts.Application.Commands.Workouts.CreateWorkout;

public class CreateWorkoutCommandHandler : IRequestHandler<CreateWorkoutCommand, Result>
{
    private readonly IWorkoutsRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateWorkoutCommandHandler>? _logger;

    public CreateWorkoutCommandHandler(IWorkoutsRepository repository, IMapper mapper, ILogger<CreateWorkoutCommandHandler>? logger = null)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result> Handle(CreateWorkoutCommand request, CancellationToken cancellationToken)
    {
        Workout workout = _mapper.Map<Workout>(request);

        _logger?.LogDebug("Attempting to create workout: {@Workout}", workout);

        if (workout == null)
        {
            _logger?.LogWarning("Workout is null");
            return Result.Invalid("Workout must not be null");
        }

        Result result = await _repository.Add(workout);

        if (result.IsSuccess)
        {
            _logger?.LogInformation("Workout created successfully: {@Workout}", workout);
        }

        return result;
    }
}