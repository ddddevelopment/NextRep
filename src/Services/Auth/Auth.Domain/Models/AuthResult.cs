namespace Auth.Domain.Models;

public class AuthResult
{
    public bool IsSuccess { get; set; }
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public int ExpiresIn { get; set; }
    public string ErrorMessage { get; set; }
}
