namespace Workouts.Api.Models;

public class SetCreateRequestForExercise
{
    public int Reps { get; set; }
    public int Weight { get; set; }
    public string? Notes { get; set; }
}
