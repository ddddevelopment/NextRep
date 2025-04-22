namespace Auth.Domain.Models;

public class TokenValidationResult {
    public bool Valid { get; set; }
    public Guid UserId { get; set; }
    public Dictionary<string, string> Claims { get; set; }
}