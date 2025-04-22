namespace Auth.Domain.Models;

public class AuthResult {
    public bool Success { get; set; }
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public int ExpiresIn { get; set; }
    public string ErrorMessage { get; set; }
    public static AuthResult CreateSuccess(string accessToken, string refreshToken, int expiresIn) {
        return new AuthResult {
            Success = true,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresIn = expiresIn
        };
    } 
    public static AuthResult CreateFailed(string errorMessage) {
        return new AuthResult {
            Success = false,
            ErrorMessage = errorMessage
        };
    }
}