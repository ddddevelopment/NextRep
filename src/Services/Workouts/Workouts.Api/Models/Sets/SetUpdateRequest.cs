namespace Workouts.Api.Models;

public class SetUpdateRequest
{
    public int Reps { get; set; }
    public int Weight { get; set; }
    public string? Notes { get; set; }
}
