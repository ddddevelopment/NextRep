namespace Auth.Domain.Models;

public class AuthResult
{
    public bool IsSuccess { get; set; }
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public int? ExpiresIn { get; set; }
    public string? ErrorMessage { get; set; }

    public static AuthResult Success(string accessToken, string refreshToken, int expiresIn) => 
        new AuthResult {
            IsSuccess = true,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresIn = expiresIn
        };

    
    public static AuthResult Failure(string errorMessage) => 
        new AuthResult {
            IsSuccess = false,
            ErrorMessage = errorMessage
        };
}
