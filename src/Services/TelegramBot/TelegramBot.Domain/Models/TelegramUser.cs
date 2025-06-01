namespace TelegramBot.Domain.Models;

public class TelegramUser
{
    public long TelegramId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Username { get; set; }
    public Guid? UserId { get; set; } // ID из микросервиса Users
    public UserState State { get; set; } = UserState.Initial;
    public string? TemporaryData { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}