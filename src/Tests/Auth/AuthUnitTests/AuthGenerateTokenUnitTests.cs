using Auth.Application.Services;

namespace AuthUnitTests;

public class AuthGenerateTokenUnitTests {
    private readonly AuthService _service;

    public AuthGenerateTokenUnitTests(AuthService service)
    {
        _service = service;
    }
    
    [Fact]
    public async Task GenerateToken_ReturnJwtToken() {
        string email = "";

        string token = await _service.GenerateToken(email);

        
    }
}