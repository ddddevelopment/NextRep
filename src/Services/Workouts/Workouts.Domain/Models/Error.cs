namespace Workouts.Domain.Models
{
    public class Error
    {
        public ErrorType Type { get; set; }
        public string? Message { get; set; }
    }
}