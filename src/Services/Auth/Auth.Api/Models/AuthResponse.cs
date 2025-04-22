namespace Auth.Api.Models;

public class AuthResponse {
    public string AccessToken { get; set; }
    public int ExpiresIn { get; set; }
    public string ErrorMessage { get; set; }
}