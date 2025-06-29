namespace TelegramBot.Domain.Models;

public class TelegramUser {
    public long Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string? LastName { get; set; }
    public string? Username { get; set; }
    public bool IsBot { get; set; }
} 