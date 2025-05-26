using MediatR;
using Workouts.Domain.Models;

namespace Workouts.Application.Commands.Workouts.CreateWorkout;

public class CreateWorkoutCommand : IRequest<Result>
{
    public Guid UserId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string? Notes { get; set; }
    public ICollection<Exercise> Exercises { get; set; } = new List<Exercise>();
}