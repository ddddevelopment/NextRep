using Auth.Domain.Models;

namespace Auth.Domain.Services;

public static class AuthResultCreator
{
    public static AuthResult CreateSuccess(string accessToken, string refreshToken, int expiresIn) {
        return new AuthResult {
            IsSuccess = true,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresIn = expiresIn
        };
    }
    
    public static AuthResult CreateFailed(string errorMessage) {
        return new AuthResult {
            IsSuccess = false,
            ErrorMessage = errorMessage
        };
    }
}

