namespace Auth.Domain.Models;

public class RefreshToken {
    public Guid Id { get; set; }
    public string Token { get; set; }
    public Guid UserId { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime ExpiryDate { get; set; }
    public bool Used { get; set; }
    public bool Revoked { get; set; }
}